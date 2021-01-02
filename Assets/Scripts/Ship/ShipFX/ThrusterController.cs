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
        _mainThruster.Strength = _shipMovement.isBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        _leftThrusterBottom.Strength = _shipMovement.isBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        _leftThrusterTop.Strength = _shipMovement.isBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        _rightThrusterBottom.Strength = _shipMovement.isBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        _rightThrusteTop.Strength = _shipMovement.isBoosting ? _rawThrottleValue: _rawThrottleValue/2;
        
        
    }

    public void SetThrusterPower()
    {
        _mainThruster.Power = _shipMovement.VelocityPercent;
        _leftThrusterBottom.Power = _shipMovement.VelocityPercent;
        _leftThrusterTop.Power = _shipMovement.VelocityPercent;
        _rightThrusterBottom.Power = _shipMovement.VelocityPercent;
        _rightThrusteTop.Power = _shipMovement.VelocityPercent;
        Debug.Log(_shipMovement.VelocityPercent);
    }
    
    public float Remap(float iMin, float iMax, float oMin, float oMax, float v)
    {
        float t = Mathf.InverseLerp(iMin, iMax, v);
        return Mathf.Lerp(oMin, oMax, t);
    }

    
}
