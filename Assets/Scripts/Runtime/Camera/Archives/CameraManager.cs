using Cinemachine;
using UnityEngine;
using System;
using UnityEngine.Windows.WebCam;

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

    GameObject target;

    //bool isAiming = false;

    void Start()
    {
        rollingCam.enabled = true;
        rollingCam.Priority = 100;
        aimingCam.enabled = true;
        aimingCam.Priority = 99;

        target = GameObject.FindGameObjectWithTag("Player");

        rollingCam.Follow = target.transform;
        rollingCam.LookAt = target.transform;
        aimingCam.Follow = target.transform;
        aimingCam.LookAt = target.transform;

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
    public void ToggleAimMode()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        aimingCam.m_XAxis.Value = rollingCam.transform.rotation.eulerAngles.y;

        rollingCam.Priority = 0;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void ToggleFollowMode(bool reset)
    {
        if (reset)
        {
            rollingCam.transform.position = aimingCam.transform.position;
            rollingCam.transform.rotation = aimingCam.transform.rotation;
        }
        
        rollingCam.Priority = 100;
    }

    /// <summary>
    /// Renvoie la direction dans laquelle la caméra actuelle regarde
    /// </summary>
    /// <returns></returns>
    public Transform GetLookingDirection()
    {
        return brain.transform;
    }
}