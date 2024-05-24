using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [HideInInspector] public float vertical;
    [HideInInspector] public float horizontal;
    [HideInInspector] public float inputX;
    [HideInInspector] public float inputY;

    void Update()
    {
        HandleInput();
    }

    public void HandleInput()
    {
        vertical = Input.GetAxisRaw("Vertical");
        horizontal = Input.GetAxisRaw("Horizontal");
        inputX = Input.GetAxisRaw("Mouse X");
        inputY = Input.GetAxisRaw("Mouse Y");
    }
}
