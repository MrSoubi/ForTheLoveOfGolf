using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    [Header("Statistics")]
    [SerializeField] CameraAngle currentAngle;

    Vector2 mouseInput;
    Vector2 minMaxY = new Vector2(-50, 50);

    [Header("References")]
    [SerializeField] Transform lookAt;
    Camera cam;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        cam.fieldOfView = currentAngle.FOV;
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
        // Valculate values
        Vector3 Direction = new Vector3(0, 0, -currentAngle.maxDistance);
        Quaternion rotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

        // Applicate values
        transform.position = lookAt.position + rotation * Direction;
        transform.LookAt(lookAt.position + currentAngle.lookAtOffset);
    }

    public void ChangeAngleSmoothly(CameraAngle newAngle, float time)
    {
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