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
    Vector3 currentPos;

    [Header("Camera shake")]
    [Range(0, 5)] public float shakeIntensity;
    [Range(.01f, .08f)] public float shakeSpeed;

    float startingIntensity, shakeTimer, shakeTimerTotal;

    [Header("References")]
    [SerializeField] Transform lookAt;
    Transform lookAtRayOrigin;
    
    Camera cam;

    bool isPaused = false;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        currentAngle = idleAngle;
        cam.fieldOfView = currentAngle.FOV;

        lookAtRayOrigin = new GameObject("LookAt_RayOrigin").transform;
        lookAtRayOrigin.SetParent(lookAt);
        lookAtRayOrigin.position = lookAt.position;

        StartCoroutine(ShakeInterpolation());
    }

    private void Update()
    {
        GetInput();
        Shake();
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
        currentPos = lookAt.position + rotation * Direction;
    }

    // WARNING ! Shange -> Change (Manu)
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

    public void SetShaking(float intensity, float time)
    {
        startingIntensity = intensity;
        shakeIntensity = intensity;

        shakeTimerTotal = time;
        shakeTimer = time;
    }
    void Shake()
    {
        if (shakeTimer > 0)
        {
            shakeTimer -= Time.deltaTime;
            shakeIntensity = Mathf.Lerp(startingIntensity, 0f, 1 - (shakeTimer / shakeTimerTotal));
        }
    }
    IEnumerator ShakeInterpolation(bool isNegative = false)
    {
        float startTime = Time.time;

        Vector3 initPos = transform.localPosition, targetPos = Random.insideUnitCircle * shakeIntensity;

        while (Time.time < startTime + shakeSpeed)
        {
            float delta = 0;
            if (!isPaused) delta = (Time.time - startTime) / shakeSpeed;

            Vector3 position = Vector3.Lerp(initPos, targetPos / 10, delta) + transform.forward;

            transform.position = position;
            transform.LookAt(lookAt.position + currentAngle.lookAtOffset);

            yield return null;
        }

        StartCoroutine(ShakeInterpolation(!isNegative));
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