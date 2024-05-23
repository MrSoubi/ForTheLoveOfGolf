using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float maxSpeed;
    public float downForceValue;
    public float dragAmount;
    public float moveSpeed;

    [Header("References")]
    private InputManager inputs;
    private Rigidbody rb;

    [Header("Private")]
    public float horizontalInput;
    public float verticalInput;
    public float downForce;

    public Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputs = GetComponent<InputManager>();
    }

    private void Update()
    {
        AddDownForce();

        HandleDirection();

        SpeedControl();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f);
    }

    private void AddDownForce()
    {
        rb.AddForce(transform.up * downForceValue * -1f, ForceMode.Acceleration);
    }

    private void HandleDirection()
    {
        verticalInput = inputs.vertical;
        horizontalInput = inputs.horizontal;
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f * (IsGrounded() ? 1f : 0.1f), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }
}