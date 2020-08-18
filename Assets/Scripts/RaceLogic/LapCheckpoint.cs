using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class LapCheckpoint : MonoBehaviour
{
    public int index = 0;
    private AudioSource _source;
    
    public AudioClip overlapSound;

    
    private void Start()
    {
        _source = GetComponent<AudioSource>();
        if(overlapSound != null)
         _source.clip = overlapSound;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponentInParent<ShipLap>())
        {
            var ship = other.GetComponentInParent<ShipLap>();
            if(ship.checkpointIndex == index + 1 || ship.checkpointIndex == index - 1)
            {
                ship.checkpointIndex = index;
                _source.Play();

                Debug.Log(index);
            }
        }
    }
}
