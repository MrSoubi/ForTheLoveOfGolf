using Cinemachine;
using UnityEngine;
using System;

public class CameraManager : MonoBehaviour
{
    [Header("Initial Var")]
    [SerializeField] CameraType cameraType;
    public CameraType Type {get {return cameraType;}}

    [Header("Virtuals Cameras References")]
    [SerializeField] CinemachineFreeLook followCam;
    [SerializeField] CinemachineFreeLook aimCam;
    [SerializeField] CinemachineBrain brain;

    public static CameraManager instance;
    void Awake()
    {
        instance = this;

        // Set cameras target
        Transform player = GameObject.FindGameObjectWithTag("Player").transform;
        followCam.LookAt = player;
        followCam.Follow = player;
        aimCam.LookAt = player;
        aimCam.Follow = player;
    }

    private void Start()
    {
        SetCameraType();
    }

    // For testing
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) SetType(CameraType.Aim);
        if (Input.GetKeyDown(KeyCode.O)) SetType(CameraType.Follow);
    }

    /// <summary>
    /// Mettre le camera type en parametre, change la camera actuel en le type voulu
    /// </summary>
    /// <param name="type"></param>
    public void SetType(CameraType type)
    {
        cameraType = type;
        SetCameraType();
    }

    void SetCameraType()
    {
        followCam.gameObject.SetActive(false);
        aimCam.gameObject.SetActive(false);

        switch(cameraType)
        {
            case CameraType.Follow:
                followCam.gameObject.SetActive(true);
                ToggleFollowMode();
                break;
            
            case CameraType.Aim:
                aimCam.gameObject.SetActive(true);
                ToggleAimMode();
                break;
        }
    }

    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    void ToggleAimMode()
    {
        aimCam.m_YAxis.Value = 0.5f;
        aimCam.m_XAxis.Value = followCam.transform.rotation.eulerAngles.y;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void ToggleFollowMode()
    {
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

public enum CameraType { Follow, Aim}

/*private static CameraManager instance = null;
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
    public void ToggleAimMode()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        aimingCam.m_XAxis.Value = rollingCam.transform.rotation.eulerAngles.y;

        rollingCam.enabled = false;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void ToggleFollowMode()
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
    }*/