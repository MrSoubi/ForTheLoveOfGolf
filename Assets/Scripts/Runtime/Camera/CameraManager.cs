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
    }

    [SerializeField] private CinemachineVirtualCamera followingCam;
    [SerializeField] private CinemachineFreeLook aimingCam;
    private CinemachineBrain brain;

    GameObject target;

    void Start()
    {
        followingCam.enabled = true;
        followingCam.Priority = 100;
        aimingCam.enabled = true;
        aimingCam.Priority = 99;

        target = GameObject.FindGameObjectWithTag("Player");

        followingCam.Follow = target.transform;
        followingCam.LookAt = target.transform;
        aimingCam.Follow = target.transform;
        aimingCam.LookAt = target.transform;

        brain = GetComponentInChildren<CinemachineBrain>();
    }

    /// <summary>
    /// Active la cam�ra donn�e en param�tre en utilisant les transitions d�finies par le SCO Main Camera Blend
    /// </summary>
    /// <param name="camera"></param>
    public void ActivateCamera(CinemachineVirtualCamera camera)
    {
        camera.enabled = true;
        camera.Priority = 101;
    }

    /// <summary>
    /// D�sactive la cam�ra en cours et retourne � la cam�ra pr�c�dente
    /// </summary>
    public void DeActivateCurrentCamera()
    {
        if (aimingCam.isActiveAndEnabled || followingCam.isActiveAndEnabled)
        {
            Debug.LogWarning("Cam�ra de suivi ou de vis�e active, aucune cam�ra externe ne peut �tre d�sactiv�e.");
            return;
        }

        brain.ActiveVirtualCamera.Priority = 0;
    }

    /// <summary>
    /// Active la cam�ra de vis�e
    /// </summary>
    public void ActivateAimMode()
    {
        aimingCam.m_YAxis.Value = 0.5f;

        aimingCam.m_XAxis.Value = followingCam.transform.rotation.eulerAngles.y;

        followingCam.Priority = 0;
    }

    /// <summary>
    /// Active la cam�ra de suivi de la balle
    /// </summary>
    public void ActivateFollowMode(bool reset)
    {
        if (reset)
        {
            followingCam.transform.position = aimingCam.transform.position;
            followingCam.transform.rotation = aimingCam.transform.rotation;
        }
        
        followingCam.Priority = 100;
    }

    /// <summary>
    /// Renvoie la direction dans laquelle la cam�ra actuelle regarde
    /// </summary>
    /// <returns>Transform de la cam�ra active</returns>
    public Transform GetLookingDirection()
    {
        return brain.transform;
    }

    /// <summary>
    /// Place la cam�ra � la position et dans la rotation d�finie par le Transform lookingDirection.
    /// Ne fonctionne que si la cam�ra active est la cam�ra de suvi de d�placement.
    /// </summary>
    /// <param name="lookingDirection"></param>
    public void SetLookingDirection(Transform lookingDirection)
    {
        if (!followingCam.isActiveAndEnabled)
        {
            return;
        }

        followingCam.transform.position = lookingDirection.position;
        followingCam.transform.rotation = lookingDirection.rotation;
    }

    /// <summary>
    /// Tourne la cam�ra dans la rotation d�finie par le Quaternion lookingDirection.
    /// Ne fonctionne que si la cam�ra active est la cam�ra de suvi de d�placement.
    /// </summary>
    /// <param name="lookingDirection"></param>
    public void SetLookingDirection(Quaternion lookingDirection)
    {
        if (!followingCam.isActiveAndEnabled)
        {
            return;
        }

        followingCam.transform.rotation = lookingDirection;
    }
}