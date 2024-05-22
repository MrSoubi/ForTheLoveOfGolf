using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static CameraController;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Parameter")]
    [SerializeField] private float speedFactor;
    [SerializeField] Vector3 gravity = Vector3.down * 5f;

    private Rigidbody rb;

    public Vector3 movementInput = Vector3.zero;
    public Vector3 direction;
    Vector3 follower;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        follower = transform.position + Vector3.up;
        
        HandleInput();

        HandleDirection();

        if (movementInput.x > 0)
        {
            rb.AddForce(direction * speedFactor, ForceMode.Acceleration);
        }
        
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void HandleInput()
    {
        movementInput = new Vector3(Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
    }

    private void HandleDirection()
    {
        if (rb.velocity.magnitude != 0)
        {
            direction = Quaternion.Euler(0f, 90f * Input.GetAxis("Horizontal"), 0f) * rb.velocity;
            direction.Normalize();
        }
        else
        {
            direction = Vector3.forward;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(follower, 0.2f);
        Gizmos.DrawLine(follower, follower + direction);
    }
}