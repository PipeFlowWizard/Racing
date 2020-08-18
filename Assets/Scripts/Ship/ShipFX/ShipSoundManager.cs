using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ShipMovement))]
public class ShipSoundManager : MonoBehaviour
{
    private ShipMovement _shipMovement;
    private int _isBoosting = 0;
    
    public AnimationCurve pitchCurve;
    public AudioSource engineSource;
    public AudioSource boostSource;
    public AudioSource brakeSource;

    // Start is called before the first frame update
    void Start()
    {
        _shipMovement = GetComponent<ShipMovement>();
        engineSource.clip = _shipMovement.stats.engineClip;
        engineSource.loop = true;

        boostSource.loop = true;
        boostSource.clip = _shipMovement.stats.boostClip;

        brakeSource.loop = true;
        brakeSource.clip = _shipMovement.stats.brakeClip;
    }

    private void FixedUpdate()
    {
        _isBoosting = _shipMovement.isBoosting;
        Throttle();
        Brake();
        Boost();
    }
    
    
    
    
    private bool _wasBoosting = false;
    
    public void Boost()
    {
        boostSource.volume = Mathf.Lerp(0, .8f, _isBoosting);
        
        if (_isBoosting > 0.1f)
        {
            

            if (_wasBoosting == false)
            {
                boostSource.enabled = true;
                //boostSource.Play();
                _wasBoosting = true;
            }

        }
        else
        {
            boostSource.enabled = false;
            _wasBoosting = false;
        }
    }

    public void Throttle()
    {
        engineSource.pitch = pitchCurve.Evaluate(_shipMovement.VelocityPercent);
        engineSource.volume = Mathf.Lerp(.1f, .7f, _shipMovement.VelocityPercent+.3f);
    }

    public void Brake()
    {
        if (_shipMovement.isDrifting)
            brakeSource.volume = .7f;
        else
            brakeSource.volume = 0;
    }
}
