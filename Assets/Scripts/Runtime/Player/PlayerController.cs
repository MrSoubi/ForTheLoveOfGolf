using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CameraController;

public class PlayerController : MonoBehaviour
{
    public bool canMove { get; private set; } = true;

    [Header("Movement Parameter")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity;

    [Header("Ground Settings")]
    [SerializeField] private LayerMask ground;
    
    public bool grounded;

    [Header("References")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Rigidbody rb;

    private Vector3 moveDirection;
    private Vector2 currentInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, transform.localScale.x * 0.5f + 0.2f, ground);

        HandleMovementInput();

        //HandleAcceleration()
        //HandleSpeed()

        ApplyFinalMovement();
    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2(moveSpeed * Input.GetAxis("Vertical"), moveSpeed * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = transform.forward * currentInput.x + transform.right * currentInput.y;
        moveDirection.y = moveDirectionY;
    }

    private void ApplyFinalMovement()
    {
        Debug.DrawRay(transform.position, moveDirection,Color.red, 2f);

        if(!grounded) moveDirection.y -= gravity * Time.deltaTime;

        /*transform.position.x += velocity.x;
        transform.position.y += velocity.y;
        transform.position.z += velocity.z;*/
    }
}