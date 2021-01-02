using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float maxFov = 90f;
    public float minFov = 60f;
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera1, virtualCamera2;
    

    [SerializeField] private ParticleSystem speedLines;

    [SerializeField] private ShipMovement shipMovement;

    private Vector3 transformLocalPosition;
    private bool _boosting = false;

    private void Start()
    {
        transformLocalPosition = speedLines.transform.localPosition;
    }

    public void OnBoost(InputAction.CallbackContext value)
    {
        if (value.started && shipMovement.CanBoost && !_boosting)
        {
            _boosting = true;
            StartCoroutine(BoostFOV());
        }

    }

    IEnumerator BoostFOV()
    {
        CinemachineTransposer transposer = virtualCamera1.GetCinemachineComponent<CinemachineTransposer>();
        float percent = 0;
        
        
        while (percent < 1)
        {
            percent += Time.deltaTime *4;
            virtualCamera1.m_Lens.FieldOfView = Mathf.Lerp(minFov,maxFov,percent);
            virtualCamera2.m_Lens.FieldOfView = Mathf.Lerp(minFov,maxFov,percent);
            transformLocalPosition.z = Mathf.Lerp(25f, 5f, percent);
            transposer.m_FollowOffset.z = Mathf.Lerp(-10, -5, percent);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        percent = 0;
        while (percent < 1)
        {
            percent += Time.deltaTime *2;
            virtualCamera1.m_Lens.FieldOfView = Mathf.Lerp(maxFov,minFov,percent);
            virtualCamera2.m_Lens.FieldOfView = Mathf.Lerp(maxFov,minFov,percent);
            transformLocalPosition.z = Mathf.Lerp(5f, 25f, percent);
            transposer.m_FollowOffset.z = Mathf.Lerp(-5, -10, percent);
            yield return null;
        }

        _boosting = false;
    }
}
