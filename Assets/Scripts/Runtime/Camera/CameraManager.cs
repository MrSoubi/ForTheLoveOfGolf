using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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

    public void UseCamera(CinemachineVirtualCamera camera)
    {

    }

    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    public void AimShoot()
    {
        rollingCam.enabled = false;
        aimingCam.enabled = true;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void RollShoot()
    {
        rollingCam.enabled = false;
        aimingCam.enabled = true;
    }

    /// <summary>
    /// Renvoie la direction dans laquelle la caméra actuelle regarde
    /// </summary>
    /// <returns></returns>
    public Vector3 GetShootingDirection()
    {
        return Vector3.zero;
    }
}
