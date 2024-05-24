using System;
using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float rotationSpeed;
    //public float dragAmount;
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
        HandleAcceleration();

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
        if (rb.velocity.magnitude > 0.01)
        {
            direction = rb.velocity.normalized;
            direction = Quaternion.AngleAxis(Input.GetAxisRaw("Mouse X") * rotationSpeed, Vector3.up) * direction;
        }
        
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

    private void HandleAcceleration()
    {
        acceleration = direction * Input.GetAxisRaw("Vertical") * moveSpeed
            + Quaternion.AngleAxis(90, Vector3.up) * direction * Input.GetAxisRaw("Horizontal") * moveSpeed / 4;

        acceleration = Vector3.ClampMagnitude(acceleration, 5f);
    }

    private void HandleForces()
    {
        rb.AddForce(gravity + normal + acceleration, ForceMode.Acceleration);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + normal);
        Gizmos.color = Color.black;
        Gizmos.DrawLine(transform.position, transform.position + gravity);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + acceleration);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + friction);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + direction);
    }
}