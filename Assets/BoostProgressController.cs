using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostProgressController : MonoBehaviour
{
    public ProgressScale boost;
    [SerializeField] private ShipMovement _shipMovement;

    // Update is called once per frame
    void Update()
    {
        boost.targetProgressPercent = _shipMovement.CurrentBoost/_shipMovement.maxBoost;
        
    }
}