using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera rollingCam;
    [SerializeField] private CinemachineFreeLook aimingCam;
    private CinemachineBrain brain;

    bool isAiming = false;

    void Start()
    {
        rollingCam.enabled = true;
        aimingCam.enabled = true;

        brain = GetComponentInChildren<CinemachineBrain>();
    }

    public void UseCamera(CinemachineVirtualCamera camera)
    {

    }

    private void Update()
    {
        Debug.Log(brain.transform.rotation.y * 180);
    }
    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    public void AimShoot()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        

        rollingCam.enabled = false;
        //aimingCam.enabled = true;
        aimingCam.m_XAxis.Value = brain.transform.rotation.y * 180; // Problème d'offset non linéaire !

        //aimingCam.m_XAxis.Value = 0f;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void RollShoot()
    {
        rollingCam.enabled = true;
        //aimingCam.enabled = false;
    }

    /// <summary>
    /// Renvoie la direction dans laquelle la caméra actuelle regarde
    /// </summary>
    /// <returns></returns>
    public Vector3 GetShootingDirection()
    {
        Transform liveCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform;
        Transform target = brain.ActiveVirtualCamera.LookAt;

        Vector3 shootingDirection = target.position - liveCamera.position;

        return shootingDirection;
    }
}
