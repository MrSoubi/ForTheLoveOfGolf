using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera rollingCam;
    [SerializeField] private CinemachineFreeLook aimingCam;

    void Start()
    {
        rollingCam.enabled = true;
        aimingCam.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rollingCam.enabled = !rollingCam.enabled;
            aimingCam.enabled = !aimingCam.enabled;
        }
    }


}
