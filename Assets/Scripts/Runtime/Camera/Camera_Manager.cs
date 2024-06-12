using Cinemachine;
using UnityEngine;
using System.Collections;
using Cinemachine.PostFX;
using UnityEngine.VFX;
using DG.Tweening;


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

    [SerializeField] float minY = 1, maxY = 6, neutralY = 3;
    float YOrientationDelta = 0;
    [SerializeField] float YSensibility = 1;
    private void Update()
    {
        if (brain.ActiveVirtualCamera.VirtualCameraGameObject == followingCam.gameObject)
        {
            // FOV
            float speed = playerController.GetVelocity().magnitude;
            followingCam.m_Lens.FieldOfView = FOVCurve.Evaluate(speed);

            // Y Orientation
            float offsetX = followingCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset.x;
            float offsetZ = followingCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset.z;

            YOrientationDelta += -Input.GetAxis("Camera Y") * Time.deltaTime * YSensibility;

            YOrientationDelta = Mathf.Clamp(YOrientationDelta, -1, 1);

            float offsetY = Mathf.Lerp(minY, maxY, Mathf.InverseLerp(-1, 1, YOrientationDelta));

            followingCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_FollowOffset = new Vector3(offsetX, offsetY, offsetZ);
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
    /// Active la caméra de suivi de la balle. Si reset est true, la caméra reprendra la position qu'elle avait avant le aim.
    /// </summary>
    /// <param name="reset"></param>
    public void ActivateFollowMode(bool reset)
    {
        if (!reset)
        {
            Quaternion direction = Quaternion.LookRotation(playerController.GetVelocity().normalized);

            followingCam.GetCinemachineComponent<CinemachineOrbitalTransposer>().m_XAxis.Value = direction.eulerAngles.y;
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

    public void Shake(float intensity)
    {
        followingCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = intensity / 2;
    }

    public IEnumerator LandingShake()
    {
        float currentFrequency = 1.5f,
              endFrequency = 0f;

        float time = 0, duration = 1;

        while (time < duration)
        {
            followingCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>().m_FrequencyGain = Mathf.Lerp(currentFrequency, endFrequency, time / duration);

            time += Time.deltaTime;

            yield return null;
        }
    }
}