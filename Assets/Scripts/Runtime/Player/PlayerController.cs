using DG.Tweening;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerControllerData PCData;

    [Header("Gizmos")]
    public bool normalNormalized;
    public bool gravityNormalized;

    [Header("Materials")]
    public Material materialOpaque;
    public Material materialTransparent;

    [Header("Other")]
    private Rigidbody rb;
    public CameraManager cameraManager;

    private Vector3 direction;
    public Vector3 gravity;
    private Vector3 normal;
    private Vector3 friction;
    private Vector3 acceleration;

    private Vector2 playerInput;
    private Vector2 mouseInput;

    public float shootingAngle;

    private int shootCharges;

    public bool isAiming;
    public bool isGrounded;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        GetComponent<MeshRenderer>().material = materialOpaque;
    }

    private void Start()
    {
        shootCharges = PCData.shootCharges;
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
            HandleRolling();
        }
    }

    private void FixedUpdate()
    {
        if (!isAiming)
        {
            HandleForces();
        }
    }

    private void HandleAiming()
    {
        Vector3 shootDirection = Quaternion.AngleAxis(shootingAngle, Vector3.right) * cameraManager.GetShootingDirection();

        if (Input.GetKeyDown(PCData.shootInput))
        {
            UnFreeze();
            rb.velocity = shootDirection * rb.velocity.magnitude;
            Shoot(shootDirection);
            isAiming = false;
            MakePlayerOpaque();
            cameraManager.RollShoot();
        }
    }

    private void HandleRolling()
    {
        HandleDirection();
        CheckGround();
        HandleGravity();
        HandleNormal();
        HandleFriction();
        HandleAcceleration();

        LimitSpeed();
    }

    private void HandleInput()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.y = Input.GetAxis("Vertical");

        mouseInput.x = Input.GetAxisRaw("Mouse X");
        mouseInput.y = Input.GetAxisRaw("Mouse Y");

        if (!isAiming && Input.GetKeyDown(PCData.aimingInput))
        {
            Freeze();
            isAiming = true;
            MakePlayerTransparent();
            cameraManager.AimShoot();
        }

        if (isAiming && Input.GetKeyUp(PCData.aimingInput))
        {
            UnFreeze();
            isAiming = false;
            MakePlayerOpaque();
            cameraManager.RollShoot();
        }
    }

    private void HandleDirection()
    {
        // La direction tourne avec le mouvement de la souris
        // Le changement de direction est plus faible quand la vitesse augmente
        float mag = Mathf.Clamp(rb.velocity.magnitude, 0.000001f, Mathf.Infinity);

        rb.velocity = Quaternion.AngleAxis(mouseInput.x * PCData.rotationSpeed / mag, Vector3.up) * rb.velocity;

        if (rb.velocity.y < 0.0001)
        {
            direction = transform.forward;
        }

        if (rb.velocity.magnitude > 0.0001)
        {
            direction = rb.velocity.normalized; // La direction de la balle est celle de la vélocité
        }
    }

    private void HandleGravity()
    {
        float yFactor = 1f;
                
        if (isGrounded)
        {
            yFactor = PCData.yCurve.Evaluate(rb.velocity.y);

            if (yFactor < 0)
            {
                Debug.LogWarning("Player Controller : Y Curve pour la définition de la gravité est inférieur à 0, vérifier la forme de la courbe.");
            }
            gravity = new Vector3(0, -PCData.gravityForce * yFactor, 0);
        }
        else
        {
            gravity = new Vector3(0, -PCData.gravityForce, 0);
        }
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
        float accelerationSpeed = (isGrounded ? PCData.moveSpeed : PCData.moveSpeed * PCData.airMultiplier) * Time.deltaTime;

        Vector3 verticalAcceleration = direction * playerInput.y * accelerationSpeed;
        Vector3 horizontalAcceleration = Quaternion.AngleAxis(90, Vector3.up) * direction * accelerationSpeed * 100f * playerInput.x; // A revoir en fonction de la vitesse de déplacement

        acceleration = Vector3.ClampMagnitude(verticalAcceleration + horizontalAcceleration, 10);
    }



    private void HandleForces()
    {
        if (onStickySurface)
        {
            gravity = Vector3.zero;
            normal = Vector3.zero;
            friction = Vector3.zero;
        }

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
                    onStickySurface = false;
                }
            }
            else
            {
                normal = Vector3.zero;
                isGrounded = false;
                onStickySurface = false;
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
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, PCData.maxSpeed);
    }

    /// <summary>
    /// Ajoute un boost de vitesse à la balle dans la direction donnée en paramètre. Si la vitesse résultante est assez grande, elle devient la nouvelle vitesse max.
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="power"></param>
    public void Boost(Vector3 direction,float power)
    {
        rb.AddForce(direction * power, ForceMode.Impulse);
        
        if (rb.velocity.magnitude > PCData.maxSpeed)
        {
            PCData.maxSpeed = rb.velocity.magnitude;
        }
    }

    #region Bump
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
    #endregion

    /// <summary>
    /// Stop tous les mouvements de la balle et la téléporte à la position donnée en paramètre.
    /// </summary>
    /// <param name="newPosition"></param>
    public void Teleport(Vector3 newPosition)
    {
        transform.position = newPosition;
        rb.velocity = Vector3.zero;
    }

    #region Freeze

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
    #endregion

    #region Block
    /// <summary>
    /// Arrête totalement la balle, elle ne subira plus l'effet d'aucune force (gravité, input, bump...)
    /// </summary>
    public void Block()
    {

    }

    /// <summary>
    /// Débloque la balle
    /// </summary>
    public void UnBlock()
    {

    }
    #endregion

    #region Getters
    public Vector3 GetVelocity()
    {
        return rb.velocity;
    }
    #endregion

    #region Material
    private void MakePlayerOpaque()
    {
        GetComponent<MeshRenderer>().material.DOFade(1, 0.5f).OnComplete(() => { GetComponent<MeshRenderer>().material = materialOpaque; });
    }

    private void MakePlayerTransparent()
    {
        GetComponent<MeshRenderer>().material = materialTransparent;
        GetComponent<MeshRenderer>().material.DOFade(0.2f, 0.5f);
    }
    #endregion

    #region Charges de tir
    /// <summary>
    /// Applique un effet de tir à la balle. S'applique uniquement si la balle dispose de charges de tir.
    /// </summary>
    /// <param name="direction"></param>
    public void Shoot(Vector3 direction)
    {
        if (shootCharges > 0)
        {
            rb.AddForce(direction * PCData.shootForce, ForceMode.Impulse);
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

        shootCharges = Mathf.Clamp(shootCharges, 0, PCData.maxShootCharges);
    }

    private bool CanShoot()
    {
        return shootCharges > 0;
    }
    #endregion

    private void OnDrawGizmos()
    {
        if(PCData.drawNormal)
        {
            Gizmos.color = PCData.normalColor;
            if(normalNormalized)
            {
                Gizmos.DrawLine(transform.position, transform.position + normal.normalized);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + normal * PCData.factor);
            }
        }

        if(PCData.drawGravity)
        {
            Gizmos.color = PCData.gravityColor;
            if (gravityNormalized)
            {
                Gizmos.DrawLine(transform.position, transform.position + gravity.normalized);
            }
            else
            {
                Gizmos.DrawLine(transform.position, transform.position + gravity * PCData.factor);
            }
        }

        if(PCData.drawAcceleration)
        {
            Gizmos.color = PCData.accelerationColor;
            Gizmos.DrawLine(transform.position, transform.position + acceleration);
        }

        if (PCData.drawFriction)
        {
            Gizmos.color = PCData.frictionColor;
            Gizmos.DrawLine(transform.position, transform.position + friction);
        }

        if (PCData.drawDirection)
        {
            Gizmos.color = PCData.directionColor;
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