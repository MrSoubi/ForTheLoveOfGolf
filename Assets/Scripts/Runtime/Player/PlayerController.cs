using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private LayerMask whatIsGround; // Ca correspond à quoi ? Revoir le nommage peut être ?

    [Header("Jump Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float airMultiplier;

    [Header("Keybinds")]
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    [Header("Camera Settings")]
    [SerializeField] Transform playerCamera;

    private bool readyToDash;
    private bool grounded;

    private float horizontalInput;
    private float verticalInput;

    private Vector3 moveDirection;
    private Vector3 delayedForceToApply;

    private Rigidbody rb;

    public PlayerState state;

    public enum PlayerState
    {
        rolling,
        shooting,
        boosting,
        slowing,
        air
    }

    public bool boosting;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        readyToDash = true;
    }

    private void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, whatIsGround);

        GetInput();
        LimitSpeed();
        StateHandler();

        //Handle Drag
        if(state == PlayerState.rolling)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void FixedUpdate()
    {
        MoveCharacter();
    }

    private void MoveCharacter()
    {
        moveDirection = playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput;
        moveDirection.y = 0f;

        if (grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        else if (!grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(dashKey) && readyToDash && grounded)
        {
            readyToDash = false;

            Boost(5, new Vector3(1,0,0));

            Invoke(nameof(ResetDash), dashCooldown);
        }
    }

    // (Manu) Possibilité d'utiliser la fonction Vector3.ClampMagnitude à la place ? https://docs.unity3d.com/ScriptReference/Vector3.ClampMagnitude.html
    private void LimitSpeed() // Revoir le nommage ? 
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    public void Boost(float intensity, Vector3 direction)
    {
        //Boost
    }

    private void Dash()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(playerCamera.transform.forward * dashForce * 10f, ForceMode.Impulse);
    }
    private void ResetDash()
    {
        readyToDash = true;
    }

    private void StateHandler()
    {
        if (boosting)
        {
            state = PlayerState.boosting;
        }
    }
}