using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float dragAmount;
    public float gravityForce;

    [Header("References")]
    private InputManager inputs;
    private Rigidbody rb;

    public bool isGrounded;

    public Vector3 direction;
    public Vector3 gravity;
    public Vector3 normal;
    public Vector3 friction;
    public Vector3 acceleration;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputs = GetComponent<InputManager>();
    }

    private void Update()
    {
        HandleDirection();
        HandleGravity();

        CheckGround();

        HandleNormal();
        HandleFriction();
        HandleInputs();

        PrintValue();
    }

    private void PrintValue()
    {
        print(rb.velocity.magnitude);
    }

    private void FixedUpdate()
    {
        HandleForces();
    }

    private void HandleDirection()
    {
        direction = rb.velocity.normalized;
    }

    private void HandleGravity()
    {
        gravity = new Vector3(0, -gravityForce, 0);
    }

    private void HandleNormal()
    {
        if (isGrounded)
        {
            normal *= gravity.magnitude;
        }
        else
        {
            normal = Vector3.zero;
        }
    }

    private void HandleFriction()
    {
        friction = Vector3.zero;
    }

    private void HandleInputs()
    {
        //
    }

    private void HandleForces()
    {
        rb.AddForce(gravity + normal, ForceMode.Acceleration);
    }

    private void CheckGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.x * 0.5f + 0.2f))
        {
            normal = hit.normal;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }
}