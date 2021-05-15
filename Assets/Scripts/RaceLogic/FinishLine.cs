using System.Collections;
using System.Collections.Generic;
using Racing.Ship;
using TMPro;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
  public int checkpointAmt;

  private void OnTriggerEnter(Collider other){
      if(other.GetComponentInParent<LapCounter>())
      {
          var ship = other.GetComponentInParent<LapCounter>();
          var shipmovement = other.GetComponentInParent<ShipMovement>();
          if(ship.checkpointIndex == checkpointAmt)
          {
              shipmovement.AddReward(200f);
              shipmovement.lapsCompleted += 1;
              ship.checkpointIndex = 0;
              ship.currentLap++;
              ship.OnLapComplete();

          }
      }
  }
}
