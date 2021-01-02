using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Thruster : MonoBehaviour
{
    

    [SerializeField]
    private float maxFlameLength = 2.5f;
    [SerializeField]
    private float basicFlameLength = 1;
   
    [Range(0,1)]
    private float _strength = 0.5f;
    private float _power = 0;
    private Vector3 scaleChange;
    

    public float Power
    {
        get => _power;
        set => _power = value;
    }

    public float Strength
    {
        get => _strength;
        set
        {
            if (value >= 0 && value <= 1)
                _strength = value;
        }
    }

    private void Start()
    {
        scaleChange = transform.localScale;
    }
    
    private void Update()
    {
        SetFlameScale();    
    }

    private void SetFlameScale()
    {
        transform.localScale = new Vector3(scaleChange.x,scaleChange.y,_power * Mathf.Lerp(basicFlameLength,maxFlameLength,_strength)); ;
    }

    
}
