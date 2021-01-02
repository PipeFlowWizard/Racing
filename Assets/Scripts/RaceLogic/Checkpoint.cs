using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    public int index = 0;


    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<LapCounter>())
        {
            var ship = other.GetComponentInParent<LapCounter>();
            if(ship.checkpointIndex == index + 1 || ship.checkpointIndex == index - 1)
            {
                ship.checkpointIndex = index;

            }
        }
    }
}
