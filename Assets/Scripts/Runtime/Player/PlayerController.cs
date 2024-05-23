using Unity.Android.Gradle.Manifest;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float dragAmount;
    public float accelerationValue;
    public float gravityForce;

    [Header("References")]
    private InputManager inputs;
    private Rigidbody rb;

    [Header("Private")]
    public float horizontalInput;
    public float verticalInput;

    public Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputs = GetComponent<InputManager>();
    }

    private void Update()
    {
        /*Debug.DrawRay(transform.position, new Vector3(0, -gravityForce, 0), Color.red);

        RaycastHit hit;

        if(Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.x * 0.5f + 1f))
        {
            Vector3 blue = new Vector3(hit.normal.x * gravityForce, (hit.normal.y * gravityForce) - hit.distance, hit.normal.z * gravityForce);
            Debug.DrawRay(transform.position, blue, Color.blue);
            Debug.DrawRay(transform.position, blue + new Vector3(0, -gravityForce, 0), Color.green);       
        }
        */
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

    private void HandleDirection()
    {
        verticalInput = inputs.vertical;
        horizontalInput = inputs.horizontal;
    }

    private void MovePlayer()
    {
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * (moveSpeed / accelerationValue) * 10f * (IsGrounded() ? 1f : 0.1f), ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
        print(flatVel.magnitude);
    }
}