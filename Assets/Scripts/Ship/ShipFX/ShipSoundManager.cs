using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShipMovement))]
public class ShipSoundManager : MonoBehaviour
{
    private ShipMovement _shipMovement;
    private bool _isBoosting = false;
    
    public AnimationCurve pitchCurve;
    public AudioSource engineSource;
    public AudioSource boostSource;
    public AudioSource brakeSource;

    [SerializeField] private ShipSounds _shipSounds;
    
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
        _isBoosting = _shipMovement.IsBoosting;
        Throttle();
        Brake();
        SetBoostLoopVolume();
    }
    
    
    
    
    public void SetBoostLoopVolume()
    {
        boostSource.volume = _isBoosting ? 0.8f : 0;
    }

    public void OnStop(InputAction.CallbackContext value)
    {
        if(value.canceled && value.duration >= 2.0f )
        {
            boostSource.PlayOneShot(_shipSounds.boostOneShot);
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
            brakeSource.volume = .8f;
        else
            brakeSource.volume = 0;
    }
}
