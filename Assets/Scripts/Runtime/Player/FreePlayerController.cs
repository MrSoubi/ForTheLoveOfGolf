using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FreePlayerController : MonoBehaviour
{
    public float mouseSensitivity;
    public float moveSpeed;
    public float accelerationRate;

    private Vector2 cameraRotation;

    bool aiming = false;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (mouseSensitivity == 0)
        {
            mouseSensitivity = 500;
        }

        if (moveSpeed == 0)
        {
            moveSpeed = 15;
        }

        if (accelerationRate == 0)
        {
            accelerationRate = 1000;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            aiming = !aiming;
        }

        if (!aiming)
        {
            moveSpeed += Input.GetAxis("Mouse ScrollWheel") * accelerationRate * Time.deltaTime;

            float mouseY = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            float mouseX = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

            float moveForward = Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
            float moveSideways = Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
            float moveUpwards = -mouseX;

            if (Input.GetMouseButton(1))
            {
                float scale = 1.0f;
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    scale = 0.05f;
                }
                transform.position += transform.up * moveUpwards * scale;
            }
            else
            {
                cameraRotation.x += mouseX;
                cameraRotation.y += mouseY;

                transform.rotation = Quaternion.Euler(cameraRotation.x, cameraRotation.y, 0);
                transform.position += transform.forward * moveForward + transform.right * moveSideways;
            }
        }
    }
}
