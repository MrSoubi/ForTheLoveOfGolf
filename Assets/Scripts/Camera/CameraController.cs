using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Statistics")]
    [SerializeField] float distance = 10.0f;
    [SerializeField] float sensivity;
    [SerializeField] Vector3 lookAtOffset;

    Vector2 mouseInput;
    Vector2 minMaxY = new Vector2(-50, 50);

    [Header("References")]
    [SerializeField] Transform lookAt;

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
        mouseInput.x += Input.GetAxis("Mouse X") * sensivity * Time.deltaTime;
        mouseInput.y += Input.GetAxis("Mouse Y") * sensivity * Time.deltaTime;
        mouseInput.y = Mathf.Clamp(mouseInput.y, minMaxY.x, minMaxY.y);
    }
    void CameraControl()
    {
        // Valculate values
        Vector3 Direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(-mouseInput.y, mouseInput.x, 0);

        // Applicate values
        transform.position = lookAt.position + rotation * Direction;
        transform.LookAt(lookAt.position + lookAtOffset);
    }
}