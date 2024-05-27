using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UIElements;

public enum EnvironmentEffect
{
    NORMAL,
    ONWOOD,
    MOIST,
    CHILLED,
    SLIMY
}

public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    public float moveSpeed;
    public float maxSpeed;
    public float airMultiplier;
    public float rotationSpeed;
    public float gravityForce;

    private EnvironmentEffect environmentEffect = EnvironmentEffect.NORMAL;

    private Rigidbody rb;

    private Vector3 direction;
    private Vector3 gravity;
    private Vector3 normal;
    private Vector3 friction;
    private Vector3 acceleration;

    private Vector2 playerInput;
    private Vector2 mouseInput;

    private bool isAiming;
    private bool isGrounded;

    // Transparence de la balle en mode Aim
    public Material materialOpaque;
    public Material materialTransparent;

    private int shootCharges;
    private int maxShootCharges;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

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
    }

    private void HandleDirection()
    {
        // La direction tourne avec le mouvement de la souris
        rb.velocity = Quaternion.AngleAxis(mouseInput.x * rotationSpeed, Vector3.up) * rb.velocity; 

        if (rb.velocity.magnitude > 0.01)
        {
            direction = rb.velocity.normalized; // La direction de la balle est celle de la vélocité
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
        switch (environmentEffect)
        {
            case EnvironmentEffect.NORMAL:
                friction = Vector3.zero;
                break;
        }
    }

    private void HandleAcceleration()
    {
        float accelerationSpeed = (isGrounded ? moveSpeed : moveSpeed * airMultiplier) * Time.deltaTime;

        Vector3 verticalAcceleration = direction * playerInput.y * accelerationSpeed;
        Vector3 horizontalAcceleration = Quaternion.AngleAxis(90, Vector3.up) * direction * accelerationSpeed * 100f * playerInput.x;

        acceleration = Vector3.ClampMagnitude(verticalAcceleration + horizontalAcceleration, 5f);
    }

    private void HandleForces()
    {
        Vector3 forces = gravity + normal + acceleration + friction;

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
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, moveSpeed);
    }

    /// <summary>
    /// Ajoute un boost de vitesse à la balle dans la direction donnée en paramètre. Si la vitesse résultante est assez grande, elle devient la nouvelle vitesse max.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="power"></param>
    public void Boost(Vector3 direction,float power)
    {
        rb.AddForce(direction * power, ForceMode.Impulse);
        
        if (rb.velocity.magnitude > maxSpeed)
        {
            maxSpeed = rb.velocity.magnitude;
        }
    }

    /// <summary>
    /// Renvoie la balle en fonction du vecteur normal à la surface sur laquelle la balle est entrée en collision.
    /// </summary>
    /// <param name="normal"></param>
    public void BumpFlipper(Vector3 normal)
    {
        float angle = Vector3.Angle(rb.velocity, normal);

        rb.velocity = Quaternion.AngleAxis(angle * 2, normal) * rb.velocity;
    }

    /// <summary>
    /// Projette la balle dans la direction donnée en paramètre, en gardant la force de la balle
    /// </summary>
    /// <param name="direction"></param>
    public void BumpTrampoline(Vector3 direction)
    {
        rb.velocity = direction.normalized * rb.velocity.magnitude;
    }

    /// <summary>
    /// Stop tous les mouvements de la balle et la téléporte à la position donnée en paramètre.
    /// </summary>
    /// <param name="newPosition"></param>
    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
        rb.velocity = Vector3.zero;
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

    /// <summary>
    /// Informe la balle qu'elle vient de rentrer dans un environnement special
    /// </summary>
    /// <param name="effect"></param>
    public void SetEnvironmentEffect(EnvironmentEffect effect)
    {

    }

    /// <summary>
    /// Informe la balle qu'elle vient de quitter un environnement special
    /// </summary>
    /// <param name="effect"></param>
    public void UnsetEnvironmentEffect(EnvironmentEffect effect)
    {

    }

    /// <summary>
    /// Applique un effet de tir à la balle. S'applique uniquement si la balle dispose de charges de tir.
    /// </summary>
    /// <param name="direction"></param>
    public void Shoot(Vector3 direction)
    {
        if (shootCharges > 0)
        {
            rb.AddForce(direction * 10f, ForceMode.Impulse);
            shootCharges--;
        }
    }

    /// <summary>
    /// Ajoute des charges de tir à la balle. La fonction gère la quantité max de charges.
    /// </summary>
    /// <param name="amount"></param>
    public void AddShootCharges(int amount)
    {
        shootCharges += amount;

        Mathf.Clamp(shootCharges, 0, maxShootCharges);
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