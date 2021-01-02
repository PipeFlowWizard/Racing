using System.Collections;
using System.Collections.Generic;
using Doozy.Engine.Progress;
using UnityEngine;

[RequireComponent(typeof(ShipMovement))]
public class ShipUIController : MonoBehaviour
{

    private ShipMovement _shipMovement;
    private ProgressorGroup _movementProgressors;
    private Progressor _velocityProgressor, _accelerationProgressor;

    // Start is called before the first frame update
    void Start()
    {
        _shipMovement = GetComponent<ShipMovement>();
        _movementProgressors = GetComponent<ProgressorGroup>();
        
        _velocityProgressor = _movementProgressors.Progressors[1];
        _accelerationProgressor = _movementProgressors.Progressors[0];
        _velocityProgressor.SetMax(_shipMovement.stats.maxVelocity);
        _accelerationProgressor.SetMax(_shipMovement.stats.maxAcceleration);
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _velocityProgressor.InstantSetValue(_shipMovement.CurrentVelocity);
        _accelerationProgressor.InstantSetValue(_shipMovement.CurrentAcceleration);
        foreach (var progressor in _movementProgressors.Progressors)
        {
            progressor.UpdateProgress();
            progressor.UpdateProgressTargets();
        }
    }
}
