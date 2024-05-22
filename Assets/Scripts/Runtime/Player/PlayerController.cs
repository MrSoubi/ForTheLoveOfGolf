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

    Vector3 movementInput = Vector3.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        HandleInput();

        rb.AddForce(movementInput * speedFactor, ForceMode.Acceleration);
        rb.AddForce(gravity, ForceMode.Acceleration);
    }

    private void HandleInput()
    {
        movementInput = new Vector3(Input.GetAxis("Vertical"), 0, -Input.GetAxis("Horizontal"));
    }
}