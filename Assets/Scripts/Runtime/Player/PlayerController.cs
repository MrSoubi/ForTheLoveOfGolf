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
    GameObject focusPoint;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        focusPoint = transform.GetChild(0).gameObject;
    }

    private void Update()
    {
        focusPoint.transform.position = transform.position + Vector3.up;
        focusPoint.transform.rotation = Quaternion.Euler(rb.velocity.x, rb.velocity.y, rb.velocity.z);

        HandleInput();

        HandleDirection();

        if (movementInput.magnitude > 0)
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
        Gizmos.DrawSphere(focusPoint.transform.position, 0.2f);
        Gizmos.DrawLine(focusPoint.transform.position, focusPoint.transform.position + direction);
    }
}