using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float rollingSpeed;
    [SerializeField] private float boostSpeedChangeFactor;
    [SerializeField] private float groundDrag;

    private float moveSpeed;
    private float boostIntensity;
    private Vector3 boostDirection;

    [Header("Ground Check")]
    [SerializeField] private LayerMask groundLayer;

    private bool grounded;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce;
    [SerializeField] private float dashCooldown;
    [SerializeField] private float airMultiplier;

    [Header("Keybinds")]
    [SerializeField] private KeyCode dashKey = KeyCode.Space;

    [Header("Camera Settings")]
    [SerializeField] Transform playerCamera;

    private Vector3 moveDirection;

    private Rigidbody rb;
    private PlayerBoost playerBoost;

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

        playerBoost = GetComponent<PlayerBoost>();
    }

    private void Update()
    {
        //Ground Check
        grounded = Physics.Raycast(transform.position, Vector3.down, transform.localScale.y * 0.5f + 0.2f, groundLayer);

        GetInput();

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

    private float horizontalInput;
    private float verticalInput;

    private void GetInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }

    public void Boost(float intensity, Vector3 direction)
    {
        boostDirection = direction;
        boostIntensity = moveSpeed * intensity;

        playerBoost.Boost(boostIntensity, boostDirection);
    }

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;
    private PlayerState lastState;
    private bool keepMomentum;

    private void StateHandler()
    {
        //Mode - boosting
        if (boosting)
        {
            state = PlayerState.boosting;
            desiredMoveSpeed = boostIntensity;
            speedChangeFactor = boostSpeedChangeFactor;
        }

        //Mode - Rolling
        else if (grounded)
        {
            state = PlayerState.rolling;
            desiredMoveSpeed = rollingSpeed;
        }

        //Mode Air
        else
        {
            state = PlayerState.air;
            desiredMoveSpeed = rollingSpeed;
        }

        bool desiredMoveSpeedHasChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if(lastState == PlayerState.boosting) keepMomentum = true;

        if (desiredMoveSpeedHasChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpModeSpeed());
            }
            else
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;
    }

    private float speedChangeFactor;

    private IEnumerator SmoothlyLerpModeSpeed()
    {
        float time = 0;
        float difference = Mathf.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while(time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    private void MoveCharacter()
    {
        moveDirection = playerCamera.transform.forward * verticalInput + playerCamera.transform.right * horizontalInput;
        moveDirection.y = 0f;

        if (state == PlayerState.boosting)
        {
            if (grounded) rb.AddForce(boostDirection * boostIntensity * 10f, ForceMode.Force);
            else if (!grounded) rb.AddForce(boostDirection * boostIntensity * 10f * airMultiplier, ForceMode.Force);
        }
        else
        {
            if (grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
            else if (!grounded) rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }
}