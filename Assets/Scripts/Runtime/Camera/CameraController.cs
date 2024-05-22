using DG.Tweening;
using UnityEngine;
using TMPro;

public class CameraController : MonoBehaviour
{
    [Header("Statistics")]
    [SerializeField] CameraAngle freeAngle;
    [SerializeField] CameraAngle shootAngle;
    [SerializeField] CameraAngle boostAngle;
    [SerializeField] float transitionTime;

    CameraAngle currentAngle;
    CameraAngleType currentAngleType = CameraAngleType.Free;

    Vector2 mouseInput;
    Vector2 minMaxY = new Vector2(-50, 50);
    Vector3 lookAtOriginVelocity = Vector3.zero;

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
        currentAngle = freeAngle;
        cam.fieldOfView = currentAngle.FOV;

        // Create GO helper
        lookAtRayOrigin = new GameObject("LookAt_RayOrigin").transform;
        lookAtRayOrigin.position = lookAt.position;
    }

    private void Update()
    {
        GetInput();
        SetLookAtOriginTransform();
    }

    private void LateUpdate()
    {
        CameraControl();
    }

    void CameraControl()
    {
        // Set ray for possibleWall
        float distance = currentAngle.maxDistance;

        lookAtRayOrigin.LookAt(transform);
        if (Physics.Raycast(lookAtRayOrigin.position, lookAtRayOrigin.forward, out RaycastHit distanceHit, distance))
            distance = Vector3.Distance(lookAtRayOrigin.position, distanceHit.point);

        // Calculate values
        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

        // Applicate values
        transform.position = lookAtRayOrigin.position + rotation * Direction;
        transform.LookAt(lookAtRayOrigin.position);
    }
    void SetLookAtOriginTransform()
    {
        lookAtRayOrigin.position = Vector3.SmoothDamp(lookAtRayOrigin.position, lookAt.position + currentAngle.lookAtOffset, ref lookAtOriginVelocity, currentAngle.timeOffset);
    }

    public CameraAngleType GetCameraAngleType() { return currentAngleType; }
    public void ChangeCameraAngle(CameraAngleType cameraAngleType)
    {
        switch (cameraAngleType)
        {
            case CameraAngleType.Free:
                ChangeAngleSmoothly(freeAngle, transitionTime);
                break;
            
            case CameraAngleType.Shoot:
                ChangeAngleSmoothly(shootAngle, transitionTime);
                break;
        }

        currentAngleType = cameraAngleType;
    }
    void ChangeAngleSmoothly(CameraAngle newAngle, float time)
    {
        DOTween.Kill(this);

        DOTween.To(() => currentAngle.maxDistance, x => currentAngle.maxDistance = x, newAngle.maxDistance, time);
        DOTween.To(() => currentAngle.sensitivity, x => currentAngle.sensitivity = x, newAngle.sensitivity, time);
        DOTween.To(() => currentAngle.timeOffset, x => currentAngle.timeOffset = x, newAngle.timeOffset, time);

        cam.DOFieldOfView(newAngle.FOV, time);
        currentAngle.FOV = newAngle.FOV;

        DOTween.To(() => currentAngle.lookAtOffset, x => currentAngle.lookAtOffset = x, newAngle.lookAtOffset, time);
    }

    void GetInput()
    {
        mouseInput.x += Input.GetAxis("Mouse X") * currentAngle.sensitivity * Time.fixedDeltaTime;
        mouseInput.y += Input.GetAxis("Mouse Y") * currentAngle.sensitivity * Time.fixedDeltaTime;
        mouseInput.y = Mathf.Clamp(mouseInput.y, minMaxY.x, minMaxY.y);
    }
}

[System.Serializable]
public class CameraAngle
{
    public float maxDistance;
    public float timeOffset;
    public float sensitivity;
    public float FOV;
    public Vector3 lookAtOffset;
}

[System.Serializable] 
public enum CameraAngleType { Free, Shoot, Boost}