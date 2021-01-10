using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriftTrails : MonoBehaviour
{
    [SerializeField] private ParticleSystem[] _driftTrails;

    private ShipMovement _shipMovement;

    private void Start()
    {
        _shipMovement = GetComponentInParent<ShipMovement>();
    }

    private void Update()
    {
        var emissionModule = _driftTrails[0].emission;
        emissionModule.enabled = _shipMovement.isDrifting;
        var emission = _driftTrails[1].emission;
        emission.enabled = _shipMovement.isDrifting;
        
    }
}
