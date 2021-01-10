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

    [SerializeField] private CinemachineVirtualCamera virtualCamera1, virtualCamera2;
    [SerializeField] private ParticleSystem speedLines;
    [SerializeField] private ShipMovement shipMovement;

    public AnimationCurve cameraFOVCurve;

    private CinemachineTransposer transposer;
    private Vector3 speedLinesTransformLocalPosition;

    private void Start()
    {
        speedLinesTransformLocalPosition = speedLines.transform.localPosition;
        transposer = virtualCamera1.GetCinemachineComponent<CinemachineTransposer>();
    }

    private void Update()
    {
        SetCameraFOV();
        SetCameraZOffset();
    }

    public void SetCameraFOV()
    {
        var percent = cameraFOVCurve.Evaluate(shipMovement.VelocityPercent);
        virtualCamera1.m_Lens.FieldOfView = Mathf.Lerp(minFov, maxFov, percent);
        virtualCamera2.m_Lens.FieldOfView = Mathf.Lerp(minFov, maxFov, percent);
    }

    public void SetCameraZOffset()
    {
        var percent = cameraFOVCurve.Evaluate(shipMovement.VelocityPercent);
        speedLinesTransformLocalPosition.z = Mathf.Lerp(25f, 5f, percent);
        transposer.m_FollowOffset.z = Mathf.Lerp(-8, -5, percent);
    }

}