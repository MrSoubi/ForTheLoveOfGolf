using Cinemachine;
using UnityEngine;
using System.Collections;
using Cinemachine.PostFX;
using UnityEngine.VFX;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance = null; 
    public static CameraManager Instance => instance;
    public bool enableBoostEffect = false;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);

            return;
        }
        else instance = this;
    }

    [Header("Ref Camera")]
    [SerializeField] private CinemachineVirtualCamera followingCam;
    [SerializeField] private CinemachineVirtualCamera boostCam;
    [SerializeField] private CinemachineFreeLook aimingCam;
    public CinemachineBrain brain;

    private GameObject target;
    private PC_MovingSphere playerController;

    [SerializeField] private AnimationCurve FOVCurve;
    [SerializeField] private VisualEffect trailEffect;

    void Start()
    {
        followingCam.enabled = true;
        followingCam.Priority = 100;
        aimingCam.enabled = true;
        aimingCam.Priority = 99;
        boostCam.enabled = true;
        boostCam.Priority = 0;

        target = GameObject.FindGameObjectWithTag("Player");

        if (target)
        {
            playerController = target.GetComponent<PC_MovingSphere>();
        }

        followingCam.Follow = target.transform;
        followingCam.LookAt = target.transform;
        aimingCam.Follow = target.transform;
        aimingCam.LookAt = target.transform;
        boostCam.Follow = target.transform;
        boostCam.LookAt = target.transform;
    }

    private void Update()
    {
        if (brain.ActiveVirtualCamera.VirtualCameraGameObject == followingCam.gameObject)
        {
            float speed = playerController.GetVelocity().magnitude;
            followingCam.m_Lens.FieldOfView = FOVCurve.Evaluate(speed);
        }

        if (trailEffect)
        {
            trailEffect.SetFloat("Speed_Lerp", playerController.GetVelocity().magnitude / playerController.GetMaxSpeedLimit());
        }
    }

    /// <summary>
    /// Active la caméra donnée en paramètre en utilisant les transitions définies par le SCO Main Camera Blend
    /// </summary>
    /// <param name="camera"></param>
    public void ActivateCamera(CinemachineVirtualCamera camera)
    {
        camera.enabled = true;
        camera.Priority = 101;
    }

    /// <summary>
    /// Désactive la caméra en cours et retourne à la caméra précédente
    /// </summary>
    public void DeActivateCurrentCamera()
    {
        if (brain.ActiveVirtualCamera.VirtualCameraGameObject == aimingCam.gameObject || brain.ActiveVirtualCamera.VirtualCameraGameObject == followingCam.gameObject)
        {
            Debug.LogWarning("Caméra de suivi ou de visée active, aucune caméra externe ne peut être désactivée.");

            return;
        }

        brain.ActiveVirtualCamera.Priority = 0;
    }

    /// <summary>
    /// Active la caméra de visée
    /// </summary>
    public void ActivateAimMode()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        aimingCam.m_XAxis.Value = followingCam.transform.rotation.eulerAngles.y;

        followingCam.Priority = 99;
        aimingCam.gameObject.GetComponent<CinemachineVolumeSettings>().enabled = true;
        aimingCam.Priority = 100;
    }

    /// <summary>
    /// Active la caméra de suivi de la balle
    /// </summary>
    public void ActivateFollowMode(bool reset)
    {
        if (reset)
        {
            followingCam.transform.position = aimingCam.transform.position;
            followingCam.transform.rotation = aimingCam.transform.rotation;
        }

        followingCam.Priority = 100;
        aimingCam.gameObject.GetComponent<CinemachineVolumeSettings>().enabled = false;
        aimingCam.Priority = 99;
    }

    /// <summary>
    /// Renvoie la direction dans laquelle la caméra actuelle regarde
    /// </summary>
    /// <returns>Transform de la caméra active</returns>
    public Transform GetLookingDirection()
    {
        return brain.transform;
    }

    /// <summary>
    /// Place la caméra à la position et dans la rotation définie par le Transform lookingDirection.
    /// Ne fonctionne que si la caméra active est la caméra de suvi de déplacement.
    /// </summary>
    /// <param name="lookingDirection"></param>
    public void SetLookingDirection(Transform lookingDirection)
    {
        if (!followingCam.isActiveAndEnabled) return;

        followingCam.transform.position = lookingDirection.position;
        followingCam.transform.rotation = lookingDirection.rotation;
    }

    /// <summary>
    /// Tourne la caméra dans la rotation définie par le Quaternion lookingDirection.
    /// Ne fonctionne que si la caméra active est la caméra de suvi de déplacement.
    /// </summary>
    /// <param name="lookingDirection"></param>
    public void SetLookingDirection(Quaternion lookingDirection)
    {
        if (!followingCam.isActiveAndEnabled) return;

        followingCam.transform.rotation = lookingDirection;
    }

    public IEnumerator BoostEffect()
    {
        if(enableBoostEffect)
        {
            followingCam.Priority--;
            aimingCam.Priority--;
            boostCam.Priority = 100;

            Debug.Log("Activation de la boost cam");

            yield return new WaitForSeconds(1);

            Debug.Log("Desactivation de la boost cam");

            followingCam.Priority++;
            aimingCam.Priority++;
            boostCam.Priority = 0;
        }
    }
}