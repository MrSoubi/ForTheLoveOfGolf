using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Statistics")]
    [SerializeField] CameraAngle idleAngle;
    [SerializeField] CameraAngle shootAngle;
    [SerializeField] float transitionTime;
    CameraAngle currentAngle;

    Vector2 mouseInput;
    Vector2 minMaxY = new Vector2(-50, 50);

    [Header("References")]
    [SerializeField] Transform lookAt;
    Transform lookAtRayOrigin;
    
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        currentAngle = idleAngle;
        cam.fieldOfView = currentAngle.FOV;

        lookAtRayOrigin = Instantiate(new GameObject("LookAt_RayOrigin"), lookAt).transform;
        lookAtRayOrigin.position = lookAt.position;
    }

    private void Update()
    {
        GetInput();
    }

    private void LateUpdate()
    {
        CameraControl();
    }

    void GetInput()
    {
        mouseInput.x += Input.GetAxis("Mouse X") * currentAngle.sensitivity * Time.deltaTime;
        mouseInput.y += Input.GetAxis("Mouse Y") * currentAngle.sensitivity * Time.deltaTime;
        mouseInput.y = Mathf.Clamp(mouseInput.y, minMaxY.x, minMaxY.y);
    }
    void CameraControl()
    {
        // Set ray for possibleWall
        float distance = currentAngle.maxDistance;

        lookAtRayOrigin.LookAt(transform);
        if(Physics.Raycast(lookAtRayOrigin.position, lookAtRayOrigin.forward, out RaycastHit hit, distance))
            distance = Vector3.Distance(lookAtRayOrigin.position, hit.point);

        // Valculate values
        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

        // Applicate values
        transform.position = lookAt.position + rotation * Direction;
        transform.LookAt(lookAt.position + currentAngle.lookAtOffset);
    }

    public void ShangeCameraAngle(CameraAngleType cameraAngleType)
    {
        switch (cameraAngleType)
        {
            case CameraAngleType.Free:
                ChangeAngleSmoothly(idleAngle, transitionTime);
                break;
            
            case CameraAngleType.Shoot:
                ChangeAngleSmoothly(shootAngle, transitionTime);
                break;
        }
    }
    void ChangeAngleSmoothly(CameraAngle newAngle, float time)
    {
        DOTween.Kill(this);

        DOTween.To(() => currentAngle.maxDistance, x => currentAngle.maxDistance = x, newAngle.maxDistance, time);
        DOTween.To(() => currentAngle.sensitivity, x => currentAngle.sensitivity = x, newAngle.sensitivity, time);

        cam.DOFieldOfView(newAngle.FOV, time);
        currentAngle.FOV = newAngle.FOV;

        DOTween.To(() => currentAngle.lookAtOffset, x => currentAngle.lookAtOffset = x, newAngle.lookAtOffset, time);
    }
}

[System.Serializable]
public class CameraAngle
{
    public float maxDistance;
    public float sensitivity;
    public float FOV;
    public Vector3 lookAtOffset;
}

[System.Serializable] 
public enum CameraAngleType { Free, Shoot}