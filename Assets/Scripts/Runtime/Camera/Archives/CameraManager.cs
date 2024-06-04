using Cinemachine;
using UnityEngine;
using System;


public class CameraManager : MonoBehaviour
{
    [Header("Initial Var")]
    [SerializeField] CameraType cameraType;
    public CameraType Type {get {return cameraType;}}

    [Header("Virtuals Cameras References")]
    [SerializeField] GameObject movementCam;
    [SerializeField] GameObject lookAtCam;
    
    public static CameraManager instance;
    void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetCameraType();
    }

    public void SetType(CameraType type)
    {
        cameraType = type;
        SetCameraType();
    }

    void SetCameraType()
    {
        movementCam.SetActive(false);
        lookAtCam.SetActive(false);

        switch(cameraType)
        {
            case CameraType.Movement:
                movementCam.SetActive(true);
                break;
            
            case CameraType.LooktAt:
                lookAtCam.SetActive(true);
                break;
        }
    }
}

public enum CameraType { Movement, LooktAt}