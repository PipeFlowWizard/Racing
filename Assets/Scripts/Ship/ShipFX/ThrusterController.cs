using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ThrusterController : MonoBehaviour
{
    public Thruster _mainThruster,_rightThrusteTop,_leftThrusterTop,_rightThrusterBottom,_leftThrusterBottom;
    private ShipMovement _shipMovement;
    private float _rawSteerValue;
    private float _rawThrottleValue;
    public float boostScaler = 2;
    public AnimationCurve thrusterOutputCurve;
    private void Start()
    {
        _shipMovement = GetComponentInParent<ShipMovement>();
    }

    private void Update()
    {
        SetThrusterPower();
        SetThrusterStrength();
    }

    public void OnSteer(InputAction.CallbackContext value )
    {
        _rawSteerValue = value.ReadValue<Vector2>().x;
    }

    public void OnThrottle(InputAction.CallbackContext value)
    {
        _rawThrottleValue = value.ReadValue<float>();
    }
    
    public void SetThrusterStrength()
    {

        _mainThruster.Strength = _shipMovement.IsBoosting ? _rawThrottleValue: _rawThrottleValue/2;

        
        if (_rawSteerValue > 0)
        {
            _leftThrusterBottom.Strength = _rawSteerValue * (_shipMovement.IsBoosting ? _rawThrottleValue : _rawThrottleValue / 1.2f);
            _leftThrusterTop.Strength = _rawSteerValue * (_shipMovement.IsBoosting ? _rawThrottleValue : _rawThrottleValue / 1.2f);  
        }
        else
        {
            _leftThrusterBottom.Strength = _shipMovement.IsBoosting ? _rawThrottleValue: _rawThrottleValue/2;
            _leftThrusterTop.Strength = _shipMovement.IsBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        }
        if (_rawSteerValue < 0)
        {
            _rightThrusterBottom.Strength = Mathf.Abs(_rawSteerValue * (_shipMovement.IsBoosting ? _rawThrottleValue : _rawThrottleValue / 1.2f));
            _rightThrusteTop.Strength = Mathf.Abs(_rawSteerValue * (_shipMovement.IsBoosting ? _rawThrottleValue : _rawThrottleValue / 1.2f));  
        }
        else
        {
            _rightThrusterBottom.Strength = _shipMovement.IsBoosting ? _rawThrottleValue: _rawThrottleValue/2;
            _rightThrusteTop.Strength = _shipMovement.IsBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        }
    }
    
    
    public void SetThrusterPower()
    {
        _mainThruster.Power = _shipMovement.VelocityPercent;
        _leftThrusterBottom.Power = _shipMovement.VelocityPercent;
        _leftThrusterTop.Power = _shipMovement.VelocityPercent;
        _rightThrusterBottom.Power = _shipMovement.VelocityPercent;
        _rightThrusteTop.Power = _shipMovement.VelocityPercent;
    }
    
    public float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }

    
}
