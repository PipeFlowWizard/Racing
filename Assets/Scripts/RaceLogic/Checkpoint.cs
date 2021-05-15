using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Racing.Ship;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index = 0;


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<LapCounter>())
        {
            var ship = other.GetComponentInParent<LapCounter>();
            var shipmovement = other.GetComponentInParent<ShipMovement>();
            if(ship.checkpointIndex == index + 1)
            {
                ship.checkpointIndex = index;
                shipmovement.AddReward(-1f);
                Debug.Log("Going backwards");
                shipmovement.checkpointsPassed -= 1;
                shipmovement.EndEpisode();

            }
            else if ( ship.checkpointIndex == index - 1)
            {
                Debug.Log("Going forwards");
                shipmovement.AddReward(1f);
                shipmovement.checkpointsPassed += 1;
                ship.checkpointIndex = index;
            }
        }
    }
}
