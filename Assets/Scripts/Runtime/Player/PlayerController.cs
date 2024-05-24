using NUnit.Framework;
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
    }

    private void FixedUpdate()
    {
        HandleForces();
    }

    private void HandleDirection()
    {
        float inputMouse = Input.GetAxisRaw("Mouse X");
        if (inputMouse != 0f)
        {
            float angle = Input.GetAxisRaw("Mouse X") * rotationSpeed;
            rb.velocity = Quaternion.AngleAxis(angle, Vector3.up) * rb.velocity; // La direction tourne avec le mouvement de la souris
        }

        if (rb.velocity.magnitude > 0.01)
        {
            direction = rb.velocity.normalized; // La direction de la balle est celle de la vélocité
        }

        Debug.Assert(direction.magnitude > 0f);
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
        acceleration = direction * Input.GetAxisRaw("Vertical") * moveSpeed * Time.deltaTime; // Avance

        acceleration += Quaternion.AngleAxis(90, Vector3.up) * direction * moveSpeed / 2 * Input.GetAxisRaw("Horizontal") * Time.deltaTime; // Strafe

        acceleration = Vector3.ClampMagnitude(acceleration, 2f);
    }

    private void HandleForces()
    {
        Vector3 forces = (gravity + normal + acceleration + friction);

        rb.AddForce(forces, ForceMode.Acceleration);
    }

    float groundDetectionLength = 0.03f;
    private void CheckGround()
    {
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.x * 0.5f + groundDetectionLength))
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
        //Gizmos.DrawLine(transform.position + direction, transform.position + direction + debugDirection);
    }
}