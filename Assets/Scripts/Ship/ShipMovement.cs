using System;
using Cinemachine;
using Shapes;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Racing.Ship
{
    public class ShipMovement : Agent
    {
        #region Variables

        //Dependencies
        //******************************************************************************************************************
        public ShipStats stats;
        private Rigidbody _rb;
        public AnimationCurve turnSpeedVsVelocityCurve;
        private CinemachineImpulseSource _impulseSource;
        public CinemachineVirtualCamera _camera;

        //Properties
        //******************************************************************************************************************
        public float VelocityPercent => _currentVelocity / stats.maxVelocity;
        public float AccelerationPercent => stats.accelerationCurve.Evaluate(VelocityPercent);
        public float CurrentVelocity => _currentVelocity;


        public Vector3 Velocity
        {
            get => _rb.velocity;
        }

        public float CurrentAcceleration => _currentAcceleration;
        public bool CanBoost => _canBoost && _currentBoost > 0;
        public float BoostModifier
        {
            get => boostModifier;
        }
        public bool IsBoosting
        {
            get => isBoosting;
            set => isBoosting = _boostHeld || value;
        }
        public float CurrentBoost
        {
            get => _currentBoost;
            set
            {
                if (value <= 0) _currentBoost = 0f;
                if (value >= maxBoost) _currentBoost = maxBoost;
                else _currentBoost = value;
            }
        }
        public State CurrentState => _currentState;
        
        //Variables - Fields
        //******************************************************************************************************************
        private float _turningSpeed = 1f;

        [Range(0f,0.05f)]
        public float driftThreshold = 0.025f;

        [SerializeField]
        private float boostModifier = 1.2f;

        public float maxBoost;
        private float _currentBoost;

        //State stuff
        //******************************************************************************************************************
        [HideInInspector] public bool isBoosting = false;
        //[HideInInspector] public bool isLaunching = false;
        private float _throttlePercent;
        private float _currentVelocity = 0f;
        private float _currentAcceleration;
        private State _currentState;
        private Vector2 _modifiedSteerValue;
        private Vector2 _rawSteerValue;
        private Vector2 _rollValue;

        public float lastDriftTime = 0;
        public State[] availableStates;
        public int checkpointsPassed = 14;
        public int lapsCompleted = 0;

        //Timers/Cooldown Flags
        //******************************************************************************************************************
        private bool _canBoost = true;
        private bool _boostHeld = false;
        
        //Enums
        //******************************************************************************************************************
        private enum DriftDirection { RIGHT,LEFT,STRAIGHT }

        private DriftDirection _mySteerDirection = DriftDirection.STRAIGHT;
        
        #endregion
        
        //Functions
        //******************************************************************************************************************

        //TODO: Rewrite class using the state pattern, overload functions to allow for mlagents implementation
        
        #region UnityCallbacks

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        
        void Start()
        {
            _startPosition = transform.position;
            _startRotation = transform.rotation;
            _rb = GetComponent<Rigidbody>();
            _impulseSource = GetComponent<CinemachineImpulseSource>();
            _rb.inertiaTensor = new Vector3(150,150,150);
            _currentBoost = maxBoost;

            availableStates = new State[]
            {
                new DefaultState(this, _rb),
                new IddleState(this, _rb),
                new DriftState(this, _rb),
                
            };
            _currentState = availableStates[0];
        }
        
        void FixedUpdate()
        {
            _currentState.Tick();
            SetAcceleration();


            _currentVelocity = _rb.velocity.magnitude;
            // _rb.AddForce(transform.forward * (_throttlePercent * _currentAcceleration * stats.maxAcceleration), ForceMode.Acceleration);
            // _rb.maxAngularVelocity = Mathf.Lerp(_currentState.maxAngularVelocity/2, _currentState.maxAngularVelocity, VelocityPercent);
            //
            // if(VelocityPercent >= 0.1f)
            // {
            //     _turningSpeed = 1.3f * turnSpeedVsVelocityCurve.Evaluate(VelocityPercent);
            //     _rb.AddRelativeTorque(_modifiedSteerValue.y * stats.pitchSpeed * _turningSpeed,
            //         _modifiedSteerValue.x * stats.yawSpeed * _turningSpeed, 
            //         -_rollValue.x * stats.rollSpeed,
            //         ForceMode.Acceleration);
            // }
            // if (isBoosting)// && !isLaunching)
            //     CurrentBoost -= 20 * Time.deltaTime;

        }

        #endregion
        
        #region InputCallbacks
        
        public void OnThrottle(InputAction.CallbackContext value)
        {
            _throttlePercent = value.ReadValue<float>();
            if (_throttlePercent >= 0.05f)
            {
                SetState(availableStates[0]);
            }
            else
            {
                SetState(availableStates[1]);
            }
        }
        
        public void OnSteer(InputAction.CallbackContext value)
        {
            _rawSteerValue = value.ReadValue<Vector2>();
        }
        
        public void OnRoll(InputAction.CallbackContext value)
        {
            _rollValue = value.ReadValue<Vector2>();
        }
        
        public void OnBoost(InputAction.CallbackContext value)
        {
            if (value.performed && CanBoost && !isBoosting)
            {
                _impulseSource.GenerateImpulse();
                _boostHeld = true;
                IsBoosting = true;
            }
        
            if (value.canceled && isBoosting)
            {
                _boostHeld = false;
                IsBoosting = false;
            }
        
        }
        
        public void OnStop(InputAction.CallbackContext value)
        {
            var stickValue = _rawSteerValue.x;
        
            if (value.performed && value.duration <=0.1f && Time.time - lastDriftTime > 1f)
            {
                SetSteerDirection(stickValue);
                if(CanDrift()) SetState(availableStates[2]);//StartDrift();
            }
            if (value.canceled && _currentState == availableStates[2])
            {
                SetState(availableStates[0]);
                // if(value.duration >= 1.5f && _canBoost) Launch(1.0f);
            }
        }
        
        #endregion
        
        #region ML-Agents Functions

        /* Observations
         * Velocity - Vector3 - (3 observations)
         * Checkoints Passed
         */

        public override void CollectObservations(VectorSensor sensor)
        {
            // Observer ship velocity
            sensor.AddObservation(_rb.velocity);
            sensor.AddObservation(_rb.angularVelocity);
            
            // Checkpoints Passed
            sensor.AddObservation(checkpointsPassed);
            
            sensor.AddObservation(isBoosting);
            sensor.AddObservation(_currentState == availableStates[2]);
        
        
        }


        /* Actions
         * Left Stick up/down (1 Continuous action, -1 to 1)
         * Left Stick left/right (1 Continuous action, -1 to 1)
         * Right Stick left/right (1 Continuous action, -1 to 1)
         * Throttle - (1 Continuous action, 0 to 1)
         * Drift - (1 Discrete action, 0 or 1)
         * Boost - (1 Discrete action, 0 or 1)
         */

        public override void OnActionReceived(ActionBuffers actions)
        {
            if (transform.position.magnitude > 2000f)
            {
                AddReward(-5f);
                EndEpisode();
            }
            
            if(VelocityPercent > 0.4f) AddReward(VelocityPercent*0.1f);
            else AddReward(-0.1f);
            
            // Throttle
           var throttle = Mathf.Clamp(actions.ContinuousActions[3],0f,1f);
           
           _rb.AddForce(transform.forward * (throttle * _currentAcceleration * stats.maxAcceleration), ForceMode.Acceleration);
            
           // Left Stick
           var steerValue = new Vector2(Mathf.Clamp(actions.ContinuousActions[0],-1f,1f), Mathf.Clamp(actions.ContinuousActions[1],-1f,1f));
            ModifySteerDirection(steerValue);
           // Right Stick
           var rollValue = new Vector2(Mathf.Clamp(actions.ContinuousActions[2],-1f,1f),0f);
           
           if(_currentState != availableStates[1])
           {
               _turningSpeed = 1.3f * turnSpeedVsVelocityCurve.Evaluate(VelocityPercent);
               _rb.AddRelativeTorque(_modifiedSteerValue.y * stats.pitchSpeed * _turningSpeed, _modifiedSteerValue.x * stats.yawSpeed * _turningSpeed,-rollValue.x * stats.rollSpeed, ForceMode.Acceleration);
           }
           
           // Drift
           if (actions.DiscreteActions[0] == 1 && Time.time - lastDriftTime > 1f && CanDrift())
           {
               SetSteerDirection(steerValue.x);
               SetState(availableStates[2]);//StartDrift();
           }
           else if (actions.DiscreteActions[0] == 0 && _currentState == availableStates[2])
           {
               SetState(availableStates[0]);
           }
           
           // Boost
           if (actions.DiscreteActions[1] == 1)
           {
               _boostHeld = true;
               IsBoosting = true;
           }
           else if (actions.DiscreteActions[1] == 0)
           {
               _boostHeld = false;
               IsBoosting = false;
           }
           
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            var continuousActionsOut = actionsOut.ContinuousActions;
            var discreteActionsOut = actionsOut.DiscreteActions;
            continuousActionsOut[3] = _throttlePercent;
            continuousActionsOut[0] = _rawSteerValue.x;
            continuousActionsOut[1] = _rawSteerValue.y;
            continuousActionsOut[2] = _rollValue.x;
            
            discreteActionsOut[1] = isBoosting ? 1 : 0;
            discreteActionsOut[0] = _currentState == availableStates[2] ? 1 : 0;
        }

        public override void OnEpisodeBegin()
        {
            var lap = GetComponent<LapCounter>();
            lap.currentLap = 1;
            lap.checkpointIndex = 0;
            AddReward(checkpointsPassed);
            checkpointsPassed = 0;
            _rb.velocity = Vector3.zero;
            _rb.angularVelocity = Vector3.zero;
            transform.position = _startPosition;
            transform.rotation = _startRotation;
        
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if(other.gameObject.CompareTag("Untagged"))
            {
                Debug.Log("Hit boundary");
                AddReward(-8f);
                //EndEpisode();
                
            }
            else if (other.gameObject.CompareTag("Checkpoint"))
            {
                Debug.Log("Hit checkpoint");
            }
            
        }

        private void OnCollisionStay(Collision other)
        {    
            if(other.gameObject.CompareTag("Untagged"))
                AddReward(-0.1f);
        }

        #endregion
        
        #region HelperFunctions
        public void SetState(State state)
        {
            if (_currentState != null)
                _currentState.OnStateExit();

            _currentState = state;
            gameObject.name = "Vehicle - " + state.GetType().Name;

            if (_currentState != null)
                _currentState.OnStateEnter();
        }

        private void SetSteerDirection(float stickValue)
        {
            if (stickValue >= -1 * driftThreshold && stickValue <= driftThreshold)
            {
                _mySteerDirection = DriftDirection.STRAIGHT;
            }
            else if (stickValue > driftThreshold)
            {
                _mySteerDirection = DriftDirection.RIGHT;
            }
            else if (stickValue < -1 * driftThreshold)
            {
                _mySteerDirection = DriftDirection.LEFT;
            }
        }

        private void ModifySteerDirection(Vector2 vals)
        {
            if (_currentState == availableStates[2])
            {
                switch (_mySteerDirection)
                {
                    case DriftDirection.RIGHT:
                        if (vals.x >= 0f)
                            _modifiedSteerValue = vals;
                        else
                        {
                            _modifiedSteerValue = new Vector2(0, vals.y);
                        }
                        break;
                    case DriftDirection.LEFT:
                        if (vals.x <= 0f)
                            _modifiedSteerValue = vals;
                        else
                        {
                            _modifiedSteerValue = new Vector2(0, vals.y);
                        }
                        break;
                    case DriftDirection.STRAIGHT:
                        _modifiedSteerValue = vals;
                        break;
                }

            }
            else
            {
                _modifiedSteerValue = vals;
            }
        }

        public bool CanDrift()
        {
            if(VelocityPercent <= 0.25f) return false;
            if(_mySteerDirection == DriftDirection.STRAIGHT) return false;
            return true;
        }

        // public void Launch(float time) { StartCoroutine(LaunchCoroutine(time)); }


        // decouple camera control


        // Set acceleration to a percentage of the max based on the acceleration curve

        void SetAcceleration()
        {
            var evaluatedAcceleration = ((stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)));
            if (_currentBoost > 0)
            {
                var boost = boostModifier * Mathf.Lerp(boostModifier, 1f, VelocityPercent);
                _currentAcceleration = isBoosting ? evaluatedAcceleration * boost : evaluatedAcceleration;
            }
            else
            {
                _currentAcceleration = evaluatedAcceleration;
            }
        }


        //******************************************************************************************************************

        // Coroutines

        // IEnumerator LaunchCoroutine(float time)
        // {
        //     if(_canBoost)
        //     {
        //         isLaunching = true;
        //         _impulseSource.GenerateImpulse();
        //     
        //         _canBoost = false;
        //         IsBoosting = true;
        //         yield return new WaitForSeconds(time);
        //     
        //         if(!_boostHeld)
        //         {
        //             IsBoosting = false;
        //         
        //         }
        //
        //         isLaunching = false;
        //         _canBoost = true;
        //     }
        // }

        //******************************************************************************************************************
        
        #endregion
    }
}