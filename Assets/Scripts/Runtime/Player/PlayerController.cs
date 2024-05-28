using DG.Tweening;
using System;
using Unity.VisualScripting;
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
    [Header("Movement Settings")]
    public float moveSpeed;
    public float maxSpeed;
    public float airMultiplier;
    public float rotationSpeed;
    public float gravityForce;

    [Header("Shooting Settings")]
    public float shootForce;
    public int shootCharges;
    public int maxShootCharges;

    [Header("Inputs")]
    public KeyCode aimingInput = KeyCode.Mouse1;
    public KeyCode shootInput = KeyCode.Mouse0;

    [Header("Materials")]
    public Material materialOpaque;
    public Material materialTransparent;

    [Header("Other")]

    private EnvironmentEffect environmentEffect = EnvironmentEffect.NORMAL;

    private Rigidbody rb;
    public CameraManager cameraManager;

    private Vector3 direction;
    public Vector3 gravity;
    private Vector3 normal;
    private Vector3 friction;
    private Vector3 acceleration;

    private Vector2 playerInput;
    private Vector2 mouseInput;

    public bool isAiming;
    public bool isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GetComponent<MeshRenderer>().material = materialOpaque;
    }

    private void Update()
    {
        HandleInput();

        if (isAiming)
        {
            HandleAiming();
        }
        else
        {
            HandleDirection();
            HandleGravity();

            CheckGround();

            HandleNormal();
            HandleFriction();
            HandleAcceleration();

            LimitSpeed();
        }
    }

    private void FixedUpdate()
    {
        if (!isAiming)
        {
            HandleForces();
        }
    }

    private void HandleInput()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        mouseInput.x = Input.GetAxisRaw("Mouse X");
        mouseInput.y = Input.GetAxisRaw("Mouse Y");

        if (!isAiming && Input.GetKeyDown(aimingInput))
        {
            Freeze();
            cameraManager.AimShoot();
            isAiming = true;
            MakePlayerTransparent();
        }

        if (isAiming && Input.GetKeyUp(aimingInput))
        {
            UnFreeze();
            cameraManager.RollShoot();
            isAiming = false;
        }
    }

    public float shootingAngle;
    private void HandleAiming()
    {
        Vector3 shootDirection = Quaternion.AngleAxis(shootingAngle, Vector3.right) * cameraManager.GetShootingDirection();

        if (Input.GetKeyDown(shootInput))
        {
            UnFreeze();
            rb.velocity = shootDirection * rb.velocity.magnitude;
            Shoot(shootDirection);
            cameraManager.RollShoot();
            isAiming = false;
            MakePlayerOpaque();
        }
    }

    private void HandleDirection()
    {
        switch(environmentEffect){
            default:
                // La direction tourne avec le mouvement de la souris
                rb.velocity = Quaternion.AngleAxis(mouseInput.x * rotationSpeed, Vector3.up) * rb.velocity;

                if (rb.velocity.y < 0.0001)
                {
                    direction = transform.forward;
                }

                if (rb.velocity.magnitude > 0.0001)
                {
                    direction = rb.velocity.normalized; // La direction de la balle est celle de la vélocité
                }
                break;
        }

    }

    private void HandleGravity()
    {
        switch (environmentEffect)
        {
            default:
                float yFactor = 1f;
                if (isGrounded)
                {
                    //yFactor = 1 / (rb.velocity.normalized.y + 2) / 2;
                }

                gravity = new Vector3(0, -gravityForce * yFactor, 0);
                break;
        }
    }

    private void HandleNormal()
    {
        switch (environmentEffect)
        {
            default:
                if (isGrounded)
                {
                   
                    normal *= gravity.magnitude;
                }
                else
                {
                    normal = Vector3.zero;
                }
                break;
        }
    }

    private void HandleFriction()
    {
        switch (environmentEffect)
        {
            default:
                friction = Vector3.zero;
                break;
        }
    }

    private void HandleAcceleration()
    {
        switch (environmentEffect)
        {
            default:
                float accelerationSpeed = (isGrounded ? moveSpeed : moveSpeed * airMultiplier) * Time.deltaTime;

                Vector3 verticalAcceleration = direction * playerInput.y * accelerationSpeed;
                Vector3 horizontalAcceleration = Quaternion.AngleAxis(90, Vector3.up) * direction * accelerationSpeed * 100f * playerInput.x; // A revoir en fonction de la vitesse de déplacement

                acceleration = Vector3.ClampMagnitude(verticalAcceleration + horizontalAcceleration, 10);
                break;
        }
    }

    private void HandleForces()
    {
        Vector3 forces = gravity + normal + acceleration + friction;

        rb.AddForce(forces, ForceMode.Acceleration);
    }

    public Vector3 contactPoint;
    public bool complexDetection;
    public float groundDetectionLength = 0.025f;
    private void CheckGround()
    {
        if (complexDetection)
        {
            Vector3 origin = transform.position;
            float radius = transform.localScale.x * 0.5f - 0.001f;
            float maxDistance = radius + groundDetectionLength;
            RaycastHit hit;

            if(Physics.SphereCast(origin, radius, Vector3.down, out hit, maxDistance))
            {
                Debug.Log(hit.distance);
                if(hit.distance <= groundDetectionLength)
                {
                    contactPoint = hit.point;
                    Debug.Log(hit.point);
                    normal = hit.normal.normalized;
                    AddShootCharges(1);
                    isGrounded = true;
                }
                else
                {
                    normal = Vector3.zero;
                    isGrounded = false;
                }
            }
            else
            {
                normal = Vector3.zero;
                isGrounded = false;
            }
        }
        else
        {
            RaycastHit hit;

            Vector3 startingPosition = transform.position + transform.localScale.x * 0.5f * Vector3.down;

            if (Physics.Raycast(startingPosition, Vector3.down, out hit, groundDetectionLength))
            {
                normal = hit.normal;
                AddShootCharges(1);
                isGrounded = true;
            }
            else
            {
                normal = Vector3.zero;
                isGrounded = false;
            }
        }
    }

    private void LimitSpeed()
    {
        switch (environmentEffect)
        {
            default:
                rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxSpeed);
                break;
        }
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

    private Vector3 savedVelocity;
    private bool isFreezed;
    /// <summary>
    /// Bloque tous les mouvements de la balle
    /// </summary>
    public void Freeze()
    {
        savedVelocity = rb.velocity;
        rb.velocity = Vector3.zero;
        isFreezed = true;
    }

    /// <summary>
    /// Relance la balle suite à un freeze
    /// </summary>
    public void UnFreeze()
    {
        rb.velocity = savedVelocity;
        isFreezed = false;
    }

    public Vector3 GetVelocity()
    {
        return rb.velocity;
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
            rb.AddForce(direction * shootForce, ForceMode.Impulse);
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

        shootCharges = Mathf.Clamp(shootCharges, 0, maxShootCharges);
    }

    [Header("Gizmos")]
    public float factor = 0.5f;
    public bool drawNormal;
    public bool normalNormalized;
    public Color normalColor = Color.blue;
    public bool drawGravity;
    public bool gravityNormalized;
    public Color gravityColor = Color.black;
    public bool drawAcceleration;
    public Color accelerationColor = Color.red;
    public bool drawFriction;
    public Color frictionColor = Color.cyan;
    public bool drawDirection;
    public Color directionColor = Color.green;

    private void OnDrawGizmos()
    {
        if(drawNormal)
        {
            Gizmos.color = normalColor;
            if(normalNormalized)
            {
                Gizmos.DrawLine(transform.position, transform.position + normal.normalized);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + normal * factor);
            }
        }

        if(drawGravity)
        {
            Gizmos.color = gravityColor;
            if (gravityNormalized)
            {
                Gizmos.DrawLine(transform.position, transform.position + gravity.normalized);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + gravity * factor);
            }
        }

        if(drawAcceleration)
        {
            Gizmos.color = accelerationColor;
            Gizmos.DrawLine(transform.position, transform.position + acceleration);
        }

        if (drawFriction)
        {
            Gizmos.color = frictionColor;
            Gizmos.DrawLine(transform.position, transform.position + friction);
        }

        if (drawDirection)
        {
            Gizmos.color = directionColor;
            Gizmos.DrawLine(transform.position, transform.position + direction);
        }

        //Gizmos.DrawLine(transform.position, transform.position + normal + gravity);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position + rb.velocity);

        Gizmos.DrawSphere(contactPoint + Vector3.up * transform.localScale.x / 2f , transform.localScale.x * 0.5f);
        Gizmos.DrawLine(contactPoint, contactPoint + 3 * Vector3.up);
    }
}