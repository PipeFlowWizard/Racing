using System;
using System.Collections;
using System.Collections.Generic;
using Racing.Ship;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
  public void OnTriggerEnter(Collider other)
  {  

    if (other.GetComponentInParent<ShipMovement>())
    {
      var ship = other.GetComponentInParent<ShipMovement>();
      ship.CurrentBoost += ship.maxBoost / 5;
      Debug.Log("collided");
    }
    
  }
  
}