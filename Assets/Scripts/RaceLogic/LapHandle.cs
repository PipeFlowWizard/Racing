using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LapHandle : MonoBehaviour
{
  public int checkpointAmt;

  private void OnTriggerEnter(Collider other){
      if(other.GetComponentInParent<ShipLap>())
      {
          var ship = other.GetComponentInParent<ShipLap>();
          if(ship.checkpointIndex == checkpointAmt)
          {
              ship.checkpointIndex = 0;
              ship.lapNumber++;
              ship.OnLapComplete();

              Debug.Log(ship.lapNumber);
          }
      }
  }
}
