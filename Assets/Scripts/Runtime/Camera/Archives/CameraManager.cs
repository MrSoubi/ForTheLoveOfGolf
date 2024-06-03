using Cinemachine;
using UnityEngine;
using System;


public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null;
    public static CameraManager Instance => instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        DontDestroyOnLoad(this.gameObject);

        // Initialisation du Game Manager...
    }

    [SerializeField] private CinemachineVirtualCamera rollingCam;
    [SerializeField] private CinemachineFreeLook aimingCam;
    private CinemachineBrain brain;

    //bool isAiming = false;

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
        //Debug.Log(brain.transform.rotation.y * 180);
    }
    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    public void AimShoot()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        aimingCam.m_XAxis.Value = rollingCam.transform.rotation.eulerAngles.y;

        rollingCam.enabled = false;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void RollShoot()
    {
        rollingCam.enabled = true;
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
