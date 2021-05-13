using System;
using Doozy.Engine.Progress;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Shapes;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ShipMovement : MonoBehaviour
{
    //******************************************************************************************************************
    
    //Dependencies
    public ShipStats stats;
    private Rigidbody _rb;
    public AnimationCurve turnSpeedVSVelocityCurve;
    private CinemachineImpulseSource _impulseSource;

    //******************************************************************************************************************

    //Properties
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
        get => _boostModifier;
    }
    public bool IsBoosting
    {
        get => _isBoosting;
        set => _isBoosting = _boostHeld || value;
    }

    public float CurrentBoost
    {
        get => _currentBoost;
        set
        {
            if (value <= 0)
                _currentBoost = 0f;
            if (value >= maxBoost)
                _currentBoost = maxBoost;
            else
                _currentBoost = value;

        }
    }

    //******************************************************************************************************************
    //Variables - Fields
    
    private float turningSpeed = 1;
    private float _maxAngularVelocity;
    [Range(0f,0.05f)]
    public float driftThreshold = 0.025f;
    [SerializeField]
    private float _boostModifier = 1.2f;
    public float maxBoost;
    private float _currentBoost;


    //******************************************************************************************************************

    //State stuff
    [HideInInspector] public bool _isBoosting = false;
    [HideInInspector] public bool isDrifting = false;
    [HideInInspector] public bool isThrottling = false;
    [HideInInspector] public bool isLaunching = false;
    private float _throttlePercent;
    private float _currentVelocity;
    private float _currentAcceleration;
    private State _currentState;
    private Vector2 _modifiedSteerValue;
    private Vector2 _rawSteerValue;
    private Vector2 _rollValue;

    public State[] availableStates;

    //******************************************************************************************************************

    //Timers/Cooldown Flags

    private bool _canBoost = true;
    private bool _boostHeld = false;



    //******************************************************************************************************************

    //Callbacks
    
    //******************************************************************************************************************

    //Enums

    private enum DriftDirection { RIGHT,LEFT,STRAIGHT }

    private DriftDirection _mySteerDirection = DriftDirection.STRAIGHT;
    
    //******************************************************************************************************************
    


   

    //******************************************************************************************************************

    //Functions
    //TODO: Rewrite class using the state pattern, overload functions to allow for mlagents implementation
    #region UnityCallbacks
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        _maxAngularVelocity = stats.maxAngularVelocity;
        _rb.inertiaTensor = new Vector3(150,150,150);
        _currentBoost = maxBoost;

        availableStates = new State[]
        {
            new ThrottleState(this, _rb),
            new IddleState(this, _rb),
            new DriftState(this, _rb),
            new BoostState(),
            

        };
    }

    private void OnEnable()
    {
        Draw.LineGeometry = LineGeometry.Volumetric3D;
        Draw.LineThicknessSpace = ThicknessSpace.Pixels;
        Draw.LineThickness = 4;
        RenderPipelineManager.endCameraRendering += drawline;
    }


    public float accelerationMultiplier = 10f;

    void FixedUpdate()
    {
        SetAcceleration();
        
        _currentVelocity = _rb.velocity.magnitude;
        
        _rb.AddForce(transform.forward * (_throttlePercent * _currentAcceleration * stats.maxAcceleration), ForceMode.Acceleration);

        _rb.maxAngularVelocity = Mathf.Lerp(_maxAngularVelocity/2, _maxAngularVelocity, VelocityPercent);
        
        if(VelocityPercent >= 0.1f) 
            _rb.AddRelativeTorque(_modifiedSteerValue.y * stats.pitchSpeed* turningSpeed, _modifiedSteerValue.x * stats.yawSpeed * turningSpeed, -_rollValue.x * stats.rollSpeed, ForceMode.Acceleration);

        if (_isBoosting && !isLaunching)
            CurrentBoost -= 20 * Time.deltaTime;

    }

    #endregion

    #region InputCallbacks

    public void OnThrottle(InputAction.CallbackContext value)
    {
        _throttlePercent = value.ReadValue<float>();
        if (_throttlePercent >= 0.05f)
            isThrottling = true;
        else
        {
            isThrottling = false;
        }
    }

    public void OnSteer(InputAction.CallbackContext value)
    {
        _rawSteerValue = value.ReadValue<Vector2>();
        _currentState.Steer(_rawSteerValue);
        if(!isDrifting)
        {
            _maxAngularVelocity = stats.maxAngularVelocity;
            _modifiedSteerValue = _rawSteerValue;
            //turningSpeed = 1.3f * turnSpeedVSVelocityCurve.Evaluate(VelocityPercent);
            _rb.angularDrag = 2;
        }
        if (isDrifting)
        {
            _maxAngularVelocity = stats.maxAngularVelocity * driftModifier;
            switch (_mySteerDirection)
            {
                case DriftDirection.RIGHT:
                    if (_rawSteerValue.x >= 0f)
                        _modifiedSteerValue = _rawSteerValue;
                    else
                    {
                        _modifiedSteerValue = new Vector2(0, _rawSteerValue.y);
                    }
                    break;
                case DriftDirection.LEFT:
                    if (_rawSteerValue.x <= 0f)
                        _modifiedSteerValue = _rawSteerValue;
                    else
                    {
                        _modifiedSteerValue = new Vector2(0, _rawSteerValue.y);
                    }
                    break;
                case DriftDirection.STRAIGHT:
                    _modifiedSteerValue = _rawSteerValue;
                    break;
            }
            
            //turningSpeed = 1.7f * turnSpeedVSVelocityCurve.Evaluate(VelocityPercent);
            _rb.angularDrag = 1.5f;


        }
        turningSpeed = 1.3f * turnSpeedVSVelocityCurve.Evaluate(VelocityPercent);
  
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        _rollValue = value.ReadValue<Vector2>();
    }

    public void OnBoost(InputAction.CallbackContext value)
    {

        if (value.performed && CanBoost && !_isBoosting)
        {
            _impulseSource.GenerateImpulse();
            _boostHeld = true;
            IsBoosting = true;
            
        }

        if (value.canceled && _isBoosting)
        {
            _boostHeld = false;
            IsBoosting = false;
        }
        
    }

    public void OnStop(InputAction.CallbackContext value)
    {
        Debug.Log("Stop called");
        var stickValue = _rawSteerValue.x;
        
        if (value.performed && value.duration <=0.1f)
        {
            SetSteerDirection(stickValue);
            if(CanDrift()) StartDrift();
        }
        if (value.canceled && isDrifting)
        {
            EndDrift(value.duration);
            if(value.duration >= 1.5f && _canBoost) Launch(1.0f);
        }
    }

    #endregion
    
    public void SetState(State state)
    {
        if (_currentState != null)
            _currentState.OnStateExit();

        _currentState = state;
        gameObject.name = "Cube - " + state.GetType().Name;

        if (_currentState != null)
            _currentState.OnStateEnter();
    }

    public void SetSteerDirection(float stickValue)
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

    public bool CanDrift()
    {
       if(VelocityPercent <= 0.25f)
        return false;
       else if(_mySteerDirection == DriftDirection.STRAIGHT)
           return false;

       return true;
    }

    public void Launch(float time)
    {
        StartCoroutine(LaunchCoroutine(time));
    }

    // decouple camera control

    public CinemachineVirtualCamera _camera;

    public float driftModifier = 1.2f;

    public void StartDrift()
    {
        isDrifting = true; 
        //_camera.Priority = 3;
        Vector3 driftDirection = Vector3.Lerp(transform.forward, _rb.velocity.normalized, 0.5f);
        _rb.drag = .8f;



    }


    public void EndDrift(double duration)
    {
        _rb.drag = 1;
        _rb.velocity += transform.forward * _boostModifier * _rb.velocity.magnitude/10;
        //_camera.Priority = 1;
        isDrifting = false;
        if (duration >= .5f)
        {

            CurrentBoost += 20 * (float)duration;
            Debug.Log("boost: " + CurrentBoost);
            

        }
        
    }


    // Set acceleration to a percentage of the max based on the acceleration curve

    void SetAcceleration()
    {
        var evaluatedAcceleration = ((stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)));
        if (_currentBoost > 0)
        {
            var boost = _boostModifier * Mathf.Lerp(_boostModifier, 1f, VelocityPercent);
        _currentAcceleration = _isBoosting ? evaluatedAcceleration * boost : evaluatedAcceleration;
        }
        else
        {
            _currentAcceleration = evaluatedAcceleration;
        }
    }


    //******************************************************************************************************************

    // Coroutines

    IEnumerator LaunchCoroutine(float time)
    {
        if(_canBoost)
        {
            isLaunching = true;
            _impulseSource.GenerateImpulse();
            
            _canBoost = false;
            IsBoosting = true;
            yield return new WaitForSeconds(time);
            
            if(!_boostHeld)
            {
                IsBoosting = false;
                
            }

            isLaunching = false;
            _canBoost = true;
        }
    }

    //******************************************************************************************************************
    public void drawline(ScriptableRenderContext ctx,Camera cam)
    {
        RaycastHit hit;
        if(isDrifting)
        {
            if (Physics.Raycast(transform.position, transform.forward, out hit, 200))
                Shapes.Draw.Line(transform.position, transform.forward * 200 + transform.position, Color.red);
            else
                Shapes.Draw.Line(transform.position, transform.forward * 200 + transform.position, Color.green);
        }
    }
}