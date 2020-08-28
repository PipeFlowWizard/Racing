using Doozy.Engine.Progress;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipMovement : MonoBehaviour
{
    public ShipStats stats;
    private float _velocity;
    private float _acceleration;

    private float _rollSpeed;
    private float _yawSpeed;
    private float _pitchSpeed;

    private float boostCapacity = 100;

    public AnimationCurve turnSpeedCurve;

    private float turnOffset = 1;

    //Properties
    public float VelocityPercent => _velocity / stats.maxVelocity;
    public float Velocity => _velocity;
    public float Acceleration => _acceleration;

    public float turningSpeed
    {
        get => turnSpeedCurve.Evaluate(_velocity / stats.maxVelocity);
    }

    //State stuff
    public int isBoosting = 0;
    public bool isDrifting = false;

    public Rigidbody rb;
    private int boostAmount;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        _rollSpeed = stats.rollSpeed;
        _yawSpeed = stats.yawSpeed;
        _pitchSpeed = stats.pitchSpeed;
    }

    void FixedUpdate()
    {
        _acceleration = (stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)) +
                        (stats.boostModifier * stats.boostCurve.Evaluate(VelocityPercent) * isBoosting);
        _velocity = rb.velocity.magnitude;
    }
    
    public void Accelerate(float trigPercent)
    {

        // Set acceleration to a percentage of the max based on the acceleration curve
        if (!isDrifting)
            rb.AddForce(trigPercent * _acceleration * transform.forward, ForceMode.VelocityChange);

    }

    public void Stop(float lerpVal)
    {
        if (lerpVal >= 0.1)
        {
            isDrifting = true;
            if (VelocityPercent >= 0.5f)
            {
                Vector3 driftDirection = Vector3.Lerp(transform.forward, rb.velocity.normalized, 0.5f);
                rb.AddForce(lerpVal * _acceleration * 1.2f * driftDirection, ForceMode.VelocityChange);
            }

        }
        else
        {
            isDrifting = false;
        }
    }

    public void Steer(Vector2 percent)
    {
        if (!isDrifting)
        {
            turnOffset = 1;
            rb.AddRelativeTorque(_pitchSpeed * percent.y * turningSpeed,
                _yawSpeed * percent.x * turnOffset * turningSpeed, 0f);
        }
        else
        {
            turnOffset = 1.5f;
            rb.AddRelativeTorque(_pitchSpeed * percent.y * turningSpeed,
                _yawSpeed * percent.x * turnOffset * turningSpeed, 0f);
        }
    }

    public void Roll(Vector2 percent)
    {
        rb.AddRelativeTorque(0f, 0f, -percent.x * _rollSpeed);
    }

    public void Boost()
    {
        if (boostCapacity > 0 && isBoosting == 0)
        {
            boostAmount = 20;
        }
        else
        {
            boostAmount = 0;
        }
    }

}