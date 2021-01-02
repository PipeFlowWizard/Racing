using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FinishLine : MonoBehaviour
{
  public int checkpointAmt;

  private void OnTriggerEnter(Collider other){
      if(other.GetComponentInParent<LapCounter>())
      {
          var ship = other.GetComponentInParent<LapCounter>();
          if(ship.checkpointIndex == checkpointAmt)
          {
              ship.checkpointIndex = 0;
              ship.currentLap++;
              ship.OnLapComplete();

          }
      }
  }
}
