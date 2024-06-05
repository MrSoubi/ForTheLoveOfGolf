using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCameraAngleZone : MonoBehaviour
{
    [SerializeField] bool ChangeCamOnCollision;
    [SerializeField] bool lookAtTarget;
    [SerializeField] CinemachineVirtualCamera newCam;

    private void Start()
    {
        newCam.Priority = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && ChangeCamOnCollision)
        {
            if (lookAtTarget) newCam.LookAt = other.transform;
            ChangeAngle();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") && ChangeCamOnCollision)
        {
            ResetAngle();
        }
    }

    public void ChangeAngle()
    {
        CameraManager.instance.ToggleCinematic(newCam);
    }
    public void ResetAngle()
    {
        CameraManager.instance.ToggleToNotCinematic();
    }
}