using Cinemachine;
using UnityEngine;
using System;
using UnityEngine.Windows.WebCam;

public class CameraManager : MonoBehaviour
{
    [Header("Initial Var")]
    [SerializeField] CameraType cameraType;
    public CameraType Type {get {return cameraType;}}

    [Header("Virtuals Cameras References")]
    [SerializeField] CinemachineFreeLook followCam;
    [SerializeField] CinemachineFreeLook aimCam;
    [SerializeField] CinemachineBrain brain;

    bool isOnCinematic = false;
    Transform lastLookingirection, tmpTransform;
    CinemachineVirtualCamera cinematicCam;

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
        tmpTransform = new GameObject("Transform Helper === DONT TOUCH !!!").transform;
        tmpTransform.SetParent(transform);

        SetCameraType(cameraType);
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Mouse1)) ToggleAimMode();
        if(Input.GetKeyUp(KeyCode.Mouse1)) ToggleFollowMode();
    }

    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    public void ToggleAimMode()
    {
        SetCameraType(CameraType.Aim);
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void ToggleFollowMode()
    {
        SetCameraType(CameraType.Follow);
    }

    /// <summary>
    /// Active le camera manager en mode "Cinematique". Necessite la camera de la cinematiqe en reference
    /// </summary>
    /// <param name="newCam"></param>
    public void ToggleCinematic(CinemachineVirtualCamera newCam)
    {
        cinematicCam = newCam;
        SetCameraType(CameraType.Cinematic);
    }

    /// <summary>
    /// Retire le mode "Cinematique" et retourne en mode "Follow"
    /// </summary>
    /// <param name="newCam"></param>
    public void ToggleToNotCinematic()
    {
        SetCameraType(CameraType.Follow);
        isOnCinematic = false;

        cinematicCam.Priority = 0;
        cinematicCam = null;
    }

    void SetCameraType(CameraType type)
    {
        cameraType = type;

        switch (cameraType)
        {
            case CameraType.Follow:
                aimCam.Priority = 0;
                followCam.Priority = 100;
                
                //aimcam.m_yaxis.value = 0.5f;
                //aimcam.m_xaxis.value = followcam.transform.rotation.eulerangles.y;
                break;

            case CameraType.Aim:
                aimCam.Priority = 100;
                followCam.Priority = 0;
                break;

            case CameraType.Cinematic:
                cinematicCam.Priority = 110;
                followCam.Priority = 0;
                aimCam.Priority = 0;

                lastLookingirection = LookingDirection;

                isOnCinematic = true;
                break;
        }
    }

    /// <summary>
    /// Return if is on cinematic
    /// </summary>
    public bool IsOnCinematic
    {
        get
        {
            return isOnCinematic;
        }
    }

    /// <summary>
    /// Return the looking direction of the current camera
    /// </summary>
    public Transform LookingDirection
    {
        get
        {
            if (!isOnCinematic)
            {
                Vector3 liveCamera = brain.ActiveVirtualCamera.VirtualCameraGameObject.transform.position;
                Vector3 target = brain.ActiveVirtualCamera.LookAt.position;

                Vector3 shootingDirection = target - liveCamera;

                tmpTransform.position = liveCamera;
                tmpTransform.rotation = Quaternion.LookRotation(shootingDirection);

                return tmpTransform;
            }
            else
            {
                return lastLookingirection;
            }
        }
    }
}

public enum CameraType { Follow, Aim, Cinematic}

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