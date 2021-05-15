using System.Collections;
using System.Collections.Generic;
using Racing.Ship;
using UnityEngine;

public class VelocityProgressController : MonoBehaviour
{
    public ProgressScale velocity, acceleration;
    [SerializeField] private ShipMovement _shipMovement;

    // Update is called once per frame
    void Update()
    {
        velocity.targetProgressPercent = _shipMovement.VelocityPercent;
        acceleration.targetProgressPercent = _shipMovement.CurrentAcceleration / _shipMovement.stats.maxAcceleration;
    }
}
