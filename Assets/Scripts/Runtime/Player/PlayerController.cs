using DG.Tweening;
using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float maxSpeed;
    public float rotationSpeed;
    public float gravityForce;

    [SerializeField, Range(0, 90)] float maxGroundAngle = 25f; //angle max du ce qu'est un sol

    [Header("References")]
    private InputManager inputs;
    private Rigidbody rb;

    public Vector3 direction;
    public Vector3 gravity;
    public Vector3 normal;
    public Vector3 friction;
    public Vector3 acceleration;

    public Vector3 contactNormal;
    public Vector3 velocity, desiredVelocity;

    private Vector2 playerInput;
    private Vector2 mouseInput;

    public bool isGrounded;

    // Transparence de la balle en mode Aim
    public Material materialOpaque;
    public Material materialTransparent;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        inputs = GetComponent<InputManager>();

        GetComponent<MeshRenderer>().material = materialOpaque;
    }

    private void Update()
    {
        HandleInput();

        HandleDirection();
        HandleGravity();

        CheckGround();

        HandleNormal();
        HandleFriction();
        HandleAcceleration();

        LimitSpeed();
    }

    private void FixedUpdate()
    {
        HandleForces();
    }

    private void HandleInput()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        mouseInput.x = Input.GetAxisRaw("Mouse X");
        mouseInput.y = Input.GetAxisRaw("Mouse Y");

        playerInput = Vector2.ClampMagnitude(playerInput, 1);
    }

    private void HandleDirection()
    {
        // La direction tourne avec le mouvement de la souris
        rb.velocity = Quaternion.AngleAxis(mouseInput.x * rotationSpeed, Vector3.up) * rb.velocity; 

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

        acceleration += Quaternion.AngleAxis(90, Vector3.up) * direction * moveSpeed * 100 * Input.GetAxisRaw("Horizontal") * Time.deltaTime; // Strafe

        acceleration = Vector3.ClampMagnitude(acceleration, 5f);
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
        if (Physics.Raycast(transform.position, Vector3.down, out hit, transform.localScale.x * 0.5f + groundDetectionLength))
        {
            normal = hit.normal;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
    }

    private void LimitSpeed()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
    }

    public void Boost(Vector3 direction,float power)
    {
        rb.AddForce(direction * power * 5f, ForceMode.Impulse);
    }

    public void Bump(Vector3 direction, float power)
    {
        Debug.Log("Bump");
    }

    private void MakePlayerOpaque()
    {
        GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).OnComplete(() => { GetComponent<MeshRenderer>().material = materialOpaque; });
    }

    private void MakePlayerTransparent()
    {
        GetComponent<MeshRenderer>().material = materialTransparent;
        GetComponent<MeshRenderer>().material.DOFade(0.2f, 0.5f);
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