using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera rollingCam;
    [SerializeField] private CinemachineFreeLook aimingCam;

    bool isAiming = false;

    void Start()
    {
        rollingCam.enabled = true;
        aimingCam.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isAiming = !isAiming;
            rollingCam.enabled = !isAiming;
            aimingCam.enabled = isAiming;

            if (isAiming )
            {
                aimingCam.m_YAxis.Value = 0.5f;
                aimingCam.m_XAxis.Value = 0f;
            }
        }
    }


}
