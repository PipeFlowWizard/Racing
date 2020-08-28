using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipMovement))]
public class PlayerController : MonoBehaviour
{
    
    public PlayerControls controls;

    private ShipMovement _shipMovement;    public ShipStats stats;
    public Transform projectileSpawn;

    private float _throttleValue;
    private Vector2 _steerValue;
    private Vector2 _rollValue;
    private float _stopValue;
    private int _boostVal;


    // Start is called before the first frame update
    void Awake()
    {
        controls = new PlayerControls();
        //cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Followcamera>();
        _shipMovement = GetComponent<ShipMovement>();

        controls.Gameplay.Throttle.performed += ctx => _throttleValue = ctx.ReadValue<float>();
        controls.Gameplay.Throttle.canceled += ctx => _throttleValue = 0f;

        controls.Gameplay.Stop.performed += ctx => _stopValue = ctx.ReadValue<float>();
        controls.Gameplay.Stop.canceled += ctx => _stopValue = 0f;
        
        controls.Gameplay.Steer.performed += ctx => _steerValue = ctx.ReadValue<Vector2>();
        controls.Gameplay.Steer.canceled += ctx => _steerValue = Vector2.zero;

        controls.Gameplay.Roll.performed += ctx => _rollValue = ctx.ReadValue<Vector2>();
        controls.Gameplay.Roll.canceled += ctx => _rollValue = Vector2.zero;

        controls.Gameplay.Boost.performed += ctx => _boostVal = 1;
        controls.Gameplay.Boost.canceled += ctx => _boostVal = 0;
        
    }
    
    void OnEnable()
    {
        controls.Gameplay.Enable();
    }

    private void OnDisable()
    {
        controls.Gameplay.Disable();
    }


    void FixedUpdate()
    {
        Throttle(_throttleValue);
        Steer(_steerValue);
        Roll(_rollValue);
        Stop(_stopValue);

        _shipMovement.isBoosting = _boostVal;
    }
    public void Throttle(float percent)
    {
        _shipMovement.Accelerate(percent);
    }

    public void Steer(Vector2 axis)
    {
        _shipMovement.Steer(axis);
    }

    public void Roll(Vector2 axis)
    {
        _shipMovement.Roll(axis);
    }

    public void Stop(float lerpVal)
    {
        _shipMovement.Stop(lerpVal);
    }
}
