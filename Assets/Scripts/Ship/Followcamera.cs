using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Followcamera : MonoBehaviour
{
   public ShipMovement ship;
   private Camera _cam;
   public GameObject camPos;
   private Volume _postProcessing;
   public ParticleSystem speedLines;
   

   public Vector3 basePosition = new Vector3(0f,1,-6f);
   public Vector3 extendedPosition = new Vector3(0f,1,-10f);


   public int yOffset = 0;
   public int zOffset = 0;

   public int maxFov = 120;
   public int minFov = 90;

   [Range(.01f, 1)]
   public float easeVal;
//dew

    private Vector3 _sdVelocity;
    public float smoothSpeed = 0.05f;

    void Start()
    {
        _postProcessing = GetComponentInChildren<Volume>();
        _cam = GetComponentInChildren<Camera>();

    }

    // Update is called once per frame

    void FixedUpdate(){

        //_sdVelocity = _rb.velocity;
        SetPosition();
        SetRotation();
        _cam.fieldOfView = Mathf.Lerp(minFov,maxFov,ship.VelocityPercent);
        _postProcessing.profile.TryGet(out ChromaticAberration chromaticAberration);
        chromaticAberration.intensity.value = ship.VelocityPercent + 0.2f;
        if (ship.isBoosting > 0.1f)
        {
            speedLines.Play();
        }
        else
        {
            speedLines.Pause();
        }
        
    }

    public void SetPosition(){
        
        camPos.transform.localPosition = Vector3.Lerp(basePosition,extendedPosition,ship.VelocityPercent);
        
        Vector3 desiredPosition = camPos.transform.position + yOffset*Vector3.up + zOffset*Vector3.forward;

        var transform1 = transform;
        var position = transform1.position;
        position += (desiredPosition - position) * smoothSpeed;
        transform1.position = position;
    }

    public void SetRotation()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,ship.transform.rotation, easeVal);
    }
}
 