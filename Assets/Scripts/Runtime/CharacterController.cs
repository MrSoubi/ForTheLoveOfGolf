using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private LayerMask whatIsGround;

    [Header("Jump Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float airMultiplier;

    [Header("Keybinds")]
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    private bool readyToDash = true;

    private bool grounded;

    private float horizontalInput;
    private float verticalInput;
    public Vector3 moveDirection { get; private set; }
    public bool jumpTriggered { get; private set; }
    public float dashValue { get; private set; }

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, whatIsGround);

        GetInput();
        SpeedControl();

        rb.drag = grounded ? groundDrag : 0f;
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        //Calculate movement direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        if(grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(dashKey) && readyToDash && grounded)
        {
            readyToDash = false;

            Dash();

            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Dash()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.forward * dashForce * 10f, ForceMode.Impulse);
    }

    private void ResetDash()
    {
        readyToDash = true;
    }
}
