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
    public float CurrentVelocity => _currentVelocity;
    public float CurrentAcceleration => _currentAcceleration;
    public bool CanBoost => _canBoost;
    public float BoostModifier
    {
        get => _boostModifier;
        set => _boostModifier = (stats.boostModifier * stats.boostCurve.Evaluate(value));
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
            if (value >= _maxBoost)
                _currentBoost = _maxBoost;
            else
                _currentBoost = value;

        }
    }

    //******************************************************************************************************************
    //Variables - Fields
    
    private float turningSpeed = 1;
    private float maxAngularVelocity;
    [Range(0f,0.05f)]
    public float driftThreshold = 0.025f;
    private float _boostModifier;
    private float _maxBoost;
    private float _currentBoost;


    //******************************************************************************************************************

    //State stuff
    [HideInInspector] public bool _isBoosting = false;
    [HideInInspector] public bool isDrifting = false;
    [HideInInspector] public bool isThrottling = false;
    private float _throttlePercent;
    private float _currentVelocity;
    private float _currentAcceleration;
    private Vector2 _modifiedSteerValue;
    private Vector2 _rawSteerValue;
    private Vector2 _rollValue;

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
    
    #region UnityCallbacks
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        maxAngularVelocity = stats.maxAngularVelocity;
        _rb.inertiaTensor = new Vector3(150,150,150);

    }

    private void OnEnable()
    {
        Draw.LineGeometry = LineGeometry.Volumetric3D;
        Draw.LineThicknessSpace = ThicknessSpace.Pixels;
        Draw.LineThickness = 4;
        RenderPipelineManager.endCameraRendering += drawline;
    }



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

    void FixedUpdate()
    {
        SetAcceleration();
        
        _currentVelocity = _rb.velocity.magnitude;
        
        _rb.AddForce(_throttlePercent * _currentAcceleration * transform.forward, ForceMode.VelocityChange);
        if (!isDrifting)
        {
            
           _rb.velocity = (_rb.velocity + (5 * transform.forward)).normalized * _rb.velocity.magnitude;
            // rb.velocity = transform.forward * rb.velocity.magnitude;
        }

        _rb.maxAngularVelocity = Mathf.Lerp(maxAngularVelocity, maxAngularVelocity / 2, VelocityPercent);
        
        if(VelocityPercent >= 0.1f) 
            _rb.AddRelativeTorque(_modifiedSteerValue.y * stats.pitchSpeed* turningSpeed, _modifiedSteerValue.x * stats.yawSpeed * turningSpeed, -_rollValue.x * stats.rollSpeed);
        
        
        
    }

    private void Update()
    {
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
        if(!isDrifting)
        {
            _modifiedSteerValue = _rawSteerValue;
            turningSpeed = 1.2f * turnSpeedVSVelocityCurve.Evaluate(VelocityPercent);
        }
        if (isDrifting)
        {
            Debug.Log(_rb.angularVelocity.magnitude);
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
                default:
                    break;
            }
            turningSpeed = 1.7f * turnSpeedVSVelocityCurve.Evaluate(VelocityPercent);
            
            
        }
  
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        _rollValue = value.ReadValue<Vector2>();
    }
    public void OnBoost(InputAction.CallbackContext value)
    {

        if (value.performed && _canBoost && !_isBoosting)
        {
            _impulseSource.GenerateImpulse();
            _boostHeld = true;
            IsBoosting = true;
            BoostModifier = 0f;
        }

        if (value.canceled && _isBoosting)
        {
            _boostHeld = false;
            IsBoosting = false;
            BoostModifier = 1;
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
            if(value.duration >= 1.5f && CanBoost) Launch(1.0f);
        }
    }
    
    #endregion

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
       if(VelocityPercent <= 0.5f)
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
    public void StartDrift()
    {
        isDrifting = true; 
        _camera.Priority = 3;
        Vector3 driftDirection = Vector3.Lerp(transform.forward, _rb.velocity.normalized, 0.5f);
        _rb.AddForce(_currentAcceleration * 1.2f * driftDirection, ForceMode.VelocityChange);
        
        
        
    }


    public void EndDrift(double duration)
    {
        _camera.Priority = 1;
        isDrifting = false;
        if (duration >= .5f)
        {
            
            //test
            //rb.velocity = transform.forward * rb.velocity.magnitude;
            Debug.Log("aligned");
           
        }
        
    }

    // Set acceleration to a percentage of the max based on the acceleration curve


    void SetAcceleration()
    {
        var evaluatedAcceleration = ((stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)));

        _currentAcceleration = evaluatedAcceleration + BoostModifier;
    }

    //******************************************************************************************************************
    
    // Coroutines

    IEnumerator LaunchCoroutine(float time)
    {
        if(_canBoost)
        {
            _impulseSource.GenerateImpulse();
            
            _canBoost = false;
            IsBoosting = true;
            BoostModifier = 0f;
            yield return new WaitForSeconds(time);
            
            if(!_boostHeld)
            {
                BoostModifier = 1f;
                IsBoosting = false;
                
            }
            _canBoost = true;
        }
    }
 
    //******************************************************************************************************************
}