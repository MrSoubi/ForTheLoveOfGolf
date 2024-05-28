using DG.Tweening;
using System;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEditor.UIElements;
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
    [SerializeField] private PlayerControllerData PCData;

    [Header("Movement Settings")]
    private float moveSpeed;
    private float maxSpeed;
    private float airMultiplier;
    private float rotationSpeed;
    private float gravityForce;

    private AnimationCurve yCurve;

    [Header("Shooting Settings")]
    private float shootForce;
    private int shootCharges;
    private int maxShootCharges;

    [Header("Inputs")]
    private KeyCode aimingInput = KeyCode.Mouse1;
    private KeyCode shootInput = KeyCode.Mouse0;

    [Header("Materials")]
    public Material materialOpaque;
    public Material materialTransparent;

    [Header("Gizmos")]

    public bool normalNormalized;
    public bool gravityNormalized;

    private float factor;

    private bool drawNormal; 
    private Color normalColor = Color.blue;

    private bool drawGravity;  
    private Color gravityColor = Color.black;

    private bool drawAcceleration;
    private Color accelerationColor = Color.red;

    private bool drawFriction;
    private Color frictionColor = Color.cyan;

    private bool drawDirection;
    private Color directionColor = Color.green;

    [Header("Other")]

    public EnvironmentEffect environmentEffect = EnvironmentEffect.NORMAL;

    private Rigidbody rb;
    public CameraManager cameraManager;

    private Vector3 direction;
    private Vector3 gravity;
    private Vector3 normal;
    private Vector3 friction;
    private Vector3 acceleration;

    private Vector2 playerInput;
    private Vector2 mouseInput;

    private bool isAiming;
    private bool isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GetComponent<MeshRenderer>().material = materialOpaque;
    }

    private void Start()
    {
        moveSpeed = PCData.moveSpeed;
        maxSpeed = PCData.maxSpeed;
        airMultiplier = PCData.airMultiplier;
        rotationSpeed = PCData.rotationSpeed;
        gravityForce = PCData.gravityForce;
        yCurve = PCData.yCurve;

        shootForce = PCData.shootForce;
        shootCharges = PCData.shootCharges;
        maxShootCharges = PCData.maxShootCharges;

        aimingInput = PCData.aimingInput;
        shootInput = PCData.shootInput;

        factor = PCData.factor;

        drawNormal = PCData.drawNormal;
        normalColor = PCData.normalColor;
        drawGravity = PCData.drawGravity;
        gravityColor = PCData.gravityColor;
        drawAcceleration = PCData.drawAcceleration;
        accelerationColor = PCData.accelerationColor;
        drawFriction = PCData.drawFriction;
        frictionColor = PCData.frictionColor;
        drawDirection = PCData.drawDirection;
        directionColor = PCData.directionColor;
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
            isAiming = true;
            MakePlayerTransparent();
            cameraManager.AimShoot();
        }

        if (isAiming && Input.GetKeyUp(aimingInput))
        {
            UnFreeze();
            isAiming = false;
            MakePlayerOpaque();
            cameraManager.RollShoot();
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
            isAiming = false;
            MakePlayerOpaque();
            cameraManager.RollShoot();
        }
    }

    private void HandleDirection()
    {
        switch(environmentEffect){
            default:
                // La direction tourne avec le mouvement de la souris
                // Le changement de direction est plus faible quand la vitesse augmente
                rb.velocity = Quaternion.AngleAxis(mouseInput.x * rotationSpeed / rb.velocity.magnitude, Vector3.up) * rb.velocity;

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
                    if (onStickySurface)
                    {
                        gravity = contactPoint.normalized * gravityForce * -1;
                    }
                    else
                    {
                        yFactor = yCurve.Evaluate(rb.velocity.y);

                        if (yFactor < 0)
                        {
                            Debug.LogWarning("Player Controller : Y Curve pour la définition de la gravité est inférieur à 0, vérifier la forme de la courbe.");
                        }
                        gravity = new Vector3(0, -gravityForce * yFactor, 0);
                    }
                }
                else
                {
                    gravity = new Vector3(0, -gravityForce, 0);
                }

                
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

    public bool onStickySurface;
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
                if(hit.distance <= groundDetectionLength)
                {
                    contactPoint = hit.point;
                    normal = hit.normal.normalized;
                    AddShootCharges(1);
                    isGrounded = true;

                    onStickySurface = hit.collider.CompareTag("Sticky");
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

                onStickySurface = hit.collider.CompareTag("Sticky");
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
#if !UNITY_EDITOR
        //Gizmos.DrawLine(transform.position, transform.position + rb.velocity);
#endif
        //Gizmos.DrawSphere(contactPoint + Vector3.up * transform.localScale.x / 2f , transform.localScale.x * 0.5f);
       // Gizmos.DrawLine(contactPoint, contactPoint + 3 * Vector3.up);
    }
}