using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ShipStats : ScriptableObject
{
    public AudioClip engineClip;
    public AudioClip boostClip;
    public AudioClip brakeClip;



    public float maxVelocity;
    public float maxAcceleration;
    public int maxHealth;
    public int boostModifier;

    public float maxDrag;
    public float minDrag;

    public AnimationCurve accelerationCurve;
    public AnimationCurve boostCurve;
    public float yawSpeed;
    public float rollSpeed;
    public float pitchSpeed;

}