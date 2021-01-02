using Doozy.Engine.Progress;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    //******************************************************************************************************************
    
    //Dependencies
    public ShipStats stats;
    public Rigidbody rb;
    public AnimationCurve turnSpeedCurve;

    //******************************************************************************************************************

    //Properties
    public float VelocityPercent => _currentVelocity / stats.maxVelocity;
    public float CurrentVelocity => _currentVelocity;
    public float CurrentAcceleration => _currentAcceleration;
    public bool CanBoost => _canBoost;

    //******************************************************************************************************************
    //Variables - Fields
    
    private float turningSpeed = 1;
    private float maxAngularVelocity;
    [Range(0f,0.05f)]
    public float driftThreshold = 0.025f;


    //******************************************************************************************************************

    //State stuff
    public bool isBoosting = false;
    public bool isDrifting = false;
    public bool isThrottling = false;
    private float _throttlePercent;
    private float _currentVelocity;
    private float _currentAcceleration;
    private Vector2 _modifiedSteerValue;
    private Vector2 _rawSteerValue;
    private Vector2 _rollValue;

    //******************************************************************************************************************

    //Timers/Cooldown Flags

    private bool _canBoost = true;



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
        rb = GetComponent<Rigidbody>();
        maxAngularVelocity = stats.maxAngularVelocity;

    }

    void FixedUpdate()
    {
        SetAcceleration();
        
        _currentVelocity = rb.velocity.magnitude;
        
        rb.AddForce(_throttlePercent * _currentAcceleration * transform.forward, ForceMode.VelocityChange);
        if (!isDrifting)
        {
            
            rb.velocity = transform.forward * rb.velocity.magnitude;
        }

        rb.maxAngularVelocity = Mathf.Lerp(maxAngularVelocity, maxAngularVelocity / 2, VelocityPercent);
        
        if(isThrottling) 
            rb.AddRelativeTorque(_modifiedSteerValue.y * stats.pitchSpeed* turningSpeed, _modifiedSteerValue.x * stats.yawSpeed * turningSpeed, -_rollValue.x * stats.rollSpeed);
        
        
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
            turningSpeed = 1 * turnSpeedCurve.Evaluate(VelocityPercent);
        }
        if (isDrifting)
        {
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
            turningSpeed = 1.7f * turnSpeedCurve.Evaluate(VelocityPercent);
            
            
        }
  
    }

    public void OnRoll(InputAction.CallbackContext value)
    {
        _rollValue = value.ReadValue<Vector2>();
    }
    public void OnBoost(InputAction.CallbackContext value)
    {
        
        if (value.performed && _canBoost )
        {
            Boost(2f);
        }
        
    }
    
    public void OnStop(InputAction.CallbackContext value)
    {
        var stickValue = _rawSteerValue.x;
        
        if (value.performed)
        {
            SetSteerDirection(stickValue);
            if(CanDrift()) StartDrift();
        }
        if (value.canceled && isDrifting)
        {
            EndDrift();
            if(value.duration >= 1.2f) Boost(1);
        }
    }
    
    #endregion

    public void SetSteerDirection(float stickValue)
    {
        if (stickValue >= -1 * driftThreshold && stickValue <= driftThreshold)
        {
            _mySteerDirection = DriftDirection.STRAIGHT;
        }
        else if(stickValue > driftThreshold)
        {
            _mySteerDirection = DriftDirection.RIGHT;
        }
        else if(stickValue < -1 * driftThreshold)
        {
            _mySteerDirection = DriftDirection.LEFT;
        }
        Debug.Log("Steer Direction: " + _mySteerDirection.ToString());
    }

    public bool CanDrift()
    {
       if(VelocityPercent <= 0.5f)
        return false;
       else if(_mySteerDirection == DriftDirection.STRAIGHT)
           return false;

       return true;
    }


    public void Boost(float time)
    {
        isBoosting = true;
        StartCoroutine(BoostCooldown(time));
    }
    
    // decouple camera control
    public CinemachineVirtualCamera _camera; 
    public void StartDrift()
    {
        isDrifting = true; 
        _camera.Priority = 12;
        Vector3 driftDirection = Vector3.Lerp(transform.forward, rb.velocity.normalized, 0.5f);
        rb.AddForce(_currentAcceleration * 1.2f * driftDirection, ForceMode.VelocityChange);
    }

    public void EndDrift()
    {
        _camera.Priority = 1;
        isDrifting = false;
    }
    
    // Set acceleration to a percentage of the max based on the acceleration curve
    void SetAcceleration()
    {
        //var angularVelocityN = Mathf.Abs(rb.angularVelocity.y);
        var evaluatedAcceleration = ((stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)));//+ angularVelocityN);
        
        var boostModifier = (stats.boostModifier * stats.boostCurve.Evaluate(VelocityPercent) );
        _currentAcceleration = isBoosting ?  evaluatedAcceleration + boostModifier : evaluatedAcceleration;
    }

    //******************************************************************************************************************
    
    // Coroutines

    IEnumerator BoostCooldown(float time)
    {
        _canBoost = false;
        yield return new WaitForSeconds(time);
        isBoosting = false;
        _canBoost = true;
    }
 
    //******************************************************************************************************************
}