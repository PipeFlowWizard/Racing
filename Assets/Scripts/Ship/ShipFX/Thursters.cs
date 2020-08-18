using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Thursters : MonoBehaviour
{
    PlayerControls _controls;
    [SerializeField]
    public TrailRenderer trail;

    private void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        _controls = GetComponentInParent<PlayerController>().controls;
        //particles = GetComponentsInChildren<ParticleSystem>();
        trail.gameObject.SetActive(false);


        _controls.Gameplay.Boost.performed += ctx => EnableTrail();
        _controls.Gameplay.Boost.canceled += ctx => DisableTrail();

    }

    private void EnableTrail()
    {
        // foreach(var particleSystem in particles)
        trail.gameObject.SetActive(true);

    }

    private void DisableTrail()
    {
        //foreach (var particleSystem in particles)

        trail.gameObject.SetActive(false);
   
    }

    
}





