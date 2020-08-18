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

    private float turnOffset = 1;

    //Properties
    public float VelocityPercent => _velocity / stats.maxVelocity;
    public float Velocity => _velocity;
    public float Acceleration => _acceleration;
    
    //State stuff
    public int isBoosting = 0;
    public bool isDrifting = false;

    public Rigidbody rb;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        _rollSpeed = stats.rollSpeed;
        _yawSpeed = stats.yawSpeed;
        _pitchSpeed = stats.pitchSpeed;
    }

    void FixedUpdate()
    {
        _acceleration = (stats.maxAcceleration * stats.accelerationCurve.Evaluate(VelocityPercent)) + (stats.boostModifier * stats.boostCurve.Evaluate(VelocityPercent) * isBoosting);
        _velocity = rb.velocity.magnitude;
    }
    
    public void Accelerate(float trigPercent)
    {
        if(trigPercent>=0.1)
        {
            //rb.useGravity = false;
        }
        else
        {
            //rb.useGravity = true;
        }
        // Set acceleration to a percentage of the max based on the acceleration curve
        if(!isDrifting)
            rb.AddForce(trigPercent * _acceleration * transform.forward,ForceMode.VelocityChange); 
    } 

    public void Stop(float lerpVal)
    {
        if(lerpVal>=0.1)
        {
            isDrifting = true;
        }
        else
        {
            isDrifting = false;
        }
        //rb.drag = Mathf.Lerp(stats.minDrag,stats.maxDrag,lerpVal);
        //_pitchSpeed = Mathf.Lerp(stats.pitchSpeed, 1.5f * stats.pitchSpeed, lerpVal);
        //_yawSpeed = Mathf.Lerp(stats.yawSpeed, 1.5f * stats.yawSpeed, lerpVal);
        //_rollSpeed = Mathf.Lerp(stats.rollSpeed, 1.5f * stats.rollSpeed, lerpVal);

    }

    public void Steer(Vector2 percent)
    {
    
        rb.AddRelativeTorque(_pitchSpeed * percent.y * turnOffset, _yawSpeed * percent.x * turnOffset, 0f);
        //rb.transform.Rotate(_pitchSpeed * percent.y, _yawSpeed * percent.x , 0f);
    }

    public void Roll(Vector2 percent)
    {
        rb.AddRelativeTorque(0f,0f,-percent.x*_rollSpeed);
    }

}
