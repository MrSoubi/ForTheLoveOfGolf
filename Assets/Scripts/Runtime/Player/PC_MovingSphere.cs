using DG.Tweening;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using RangeAttribute = UnityEngine.RangeAttribute;
using Unity.VisualScripting;

public class PC_MovingSphere : MonoBehaviour
{


    // ----------
    // -- Tool --
    // ----------

    public PlayerControllerData PCData;

    #region TOOL PARAMETERS
    [SerializeField, Range(0f, 100f)]
    public float
        maxAcceleration = 10f,
        maxAirAcceleration = 1f;

    [SerializeField, Range(0f, 100f)]
    float shootHeight = 2f;

    [Tooltip("Nombre de tirs max avant de retomber au sol")]
    [SerializeField]
    int maxShoots = 1;

    [Tooltip("Angle maximum du sol au del� duquel la balle ne prendre plus d'acc�l�ration")]
    [SerializeField, Range(0, 90)]
    float maxGroundAngle = 25f;

    [Tooltip("Vitesse maximale au del� de laquelle la balle n'accrochera plus au sol en cas de collision avec un l�ger rebord")]
    [SerializeField, Range(0f, 100f)]
    float maxSnapSpeed = 100f;

    [Tooltip("Distance de d�tection du sol")]
    [SerializeField, Min(0f)]
    float probeDistance = 1f;

    [Tooltip("Paliers de limite de vitesse")]
    [SerializeField]
    List<float> speedLimits;

    [Tooltip("Marge de vitesse pour la détection du passage au palier précédant (ex : Palier 1 = 20, Marge = 3, si la vitesse du joueur descend en dessous de 17, il repasse au palier 0 et sera limité à une vitesse de 20")]
    [SerializeField]
    float speedLimitMargin;

    [SerializeField]
    Material rollingMaterial, aimingMaterial;

    [SerializeField]
    float shootingAngle;

    [SerializeField]
    [Tooltip("Garder les keys entre 0 et 1 en X. Les valeurs en Y peuvent varier de n'importe quelle fa�on mais devraient rester entre 1 et 2.")]
    AnimationCurve shootCurve;

    #endregion


    [SerializeField]
    [Tooltip("Utiliser la cam�ra suivant la balle")]
    Transform playerInputSpace = default;

    [SerializeField]
    [Tooltip("Mettre l'enfant Ball")]
    Transform ball = default;

    float maxSpeed;
        
    float maxClimbSpeed = 4f, maxSwimSpeed = 5f;

    float
        maxClimbAcceleration = 40f,
        maxSwimAcceleration = 5f;
    
    float maxStairsAngle = 50f; // Non utilis�
    float maxClimbAngle = 140f; // Non utilis�
    float submergenceOffset = 0.5f;
    float submergenceRange = 1f;
    float buoyancy = 1f;
    float waterDrag = 1f;
    float swimThreshold = 0.5f;

    [Tooltip("")]
    [SerializeField]
    LayerMask probeMask = -1;

    LayerMask stairsMask = -1, climbMask = -1, waterMask = 0;

    [Tooltip("")]
    [SerializeField, Min(0.1f)]
    float ballRadius = 0.5f;

    [Tooltip("Utilisé pour l'affichage de la texture")]
    [SerializeField, Min(0f)]
    float ballAlignSpeed = 180f;

    [Tooltip("Utilisé pour l'affichage de la texture")]
    [SerializeField, Min(0f)]
    float ballAirRotation = 0.5f;

    Rigidbody body, connectedBody, previousConnectedBody;
    Vector3 playerInput;
    Vector3 velocity, connectionVelocity;
    public float Velocity;
    Vector3 connectionWorldPosition, connectionLocalPosition;
    Vector3 upAxis, rightAxis, forwardAxis;
    bool desiredShoot, desiresClimbing;
    Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;
    Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;
    int groundContactCount, steepContactCount, climbContactCount;
    bool OnGround => groundContactCount > 0;
    bool OnSteep => steepContactCount > 0;
    bool Climbing => false; // climbContactCount > 0 && stepsSinceLastJump > 2;
    bool InWater => false; // submergence > 0f;
    bool Swimming => false; // submergence >= swimThreshold;
    float submergence;
    int shootPhase;
    float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    int stepsSinceLastGrounded, stepsSinceLastJump;
    MeshRenderer meshRenderer;



    public void PreventSnapToGround()
    {
        stepsSinceLastJump = -1;
    }

    void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);

        if(speedLimits.Count < 1)
        {
            Debug.LogWarning("Speed Limits doit contenir au moins 1 élément.");
        }
        else
        {
            for(int i = 1; i < speedLimits.Count; i++)
            {
                if (speedLimits[i] <= speedLimits[i - 1])
                {
                    Debug.LogWarning("Le palier " + i + " des Speed Limits est inférieur ou égal à son prédécesseur");
                }
                if (speedLimits[i] - speedLimitMargin <= speedLimits[i - 1])
                {
                    Debug.LogWarning("Speed Limit Margin est plus grand que le palier " + (i - 1));
                }
            }
        }
    }

    void Awake()
    {
        ResetMaxSpeed();
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = ball.GetComponent<MeshRenderer>();
        OnValidate();
    }

    bool isAiming;
    void Update()
    {
        if (isBlocked)
        {
            return;
        }

        if (shouldToogleRoll)
        {
            shouldToogleRoll = false;
            ToggleRoll(false);
        }
        else
        {
            if (Input.GetMouseButtonDown(1)) // Activation du mode Aim
            {
                ToggleAim();
            }
            if (isAiming && Input.GetMouseButtonUp(1)) // Desactivation du mode Aim
            {
                ToggleRoll(true);
            }
        }

        if (isAiming)
        {
            HandleAim();
        }
        else
        {
            HandleRoll();
        }

        UpdateBall();
    }

    public GameObject shootingIndicator;
    void ShowShootingIndicator()
    {
        shootingIndicator.transform.rotation = Quaternion.Euler(shootingAngle, 0, 0);
        shootingIndicator.SetActive(true);
    }

    void HideShootingIndicator()
    {
        shootingIndicator.SetActive(false);
    }

    void ToggleAim()
    {
        isAiming = true;
        meshRenderer.material = aimingMaterial;
        playerInputSpace.GetComponent<PC_OrbitCamera>().ToggleAimMode();
        Time.timeScale = 0.1f;
    }

    /// <summary>
    /// Active le mode Roll. Si reset est true, la cam�ra reprendra la place qu'elle avait lors de la d�sactivation du mode Roll.
    /// </summary>
    /// <param name="reset"></param>
    void ToggleRoll(bool reset)
    {
        Time.timeScale = 1.0f;
        isAiming = false;
        meshRenderer.material = rollingMaterial;
        playerInputSpace.GetComponent<PC_OrbitCamera>().ToggleFollowMode(reset);
    }

    void HandleAim()
    {
        desiredShoot |= Input.GetMouseButtonDown(0);
    }

    void HandleRoll()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.z = Input.GetAxis("Vertical");
        playerInput.y = Swimming ? Input.GetAxis("UpDown") : 0f;
        playerInput = Vector3.ClampMagnitude(playerInput, 1f);

        if (playerInputSpace)
        {
            rightAxis = ProjectDirectionOnPlane(playerInputSpace.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
        }
        else
        {
            rightAxis = ProjectDirectionOnPlane(Vector3.right, upAxis);
            forwardAxis = ProjectDirectionOnPlane(Vector3.forward, upAxis);
        }

        if (Swimming)
        {
            desiresClimbing = false;
        }

    }

    void UpdateBall()
    {
        Vector3 rotationPlaneNormal = lastContactNormal;
        float rotationFactor = 1f;

        if (!OnGround)
        {
            if (OnSteep)
            {
                rotationPlaneNormal = lastSteepNormal;
            }
            else
            {
                rotationFactor = ballAirRotation;
            }
        }

        Vector3 movement = (body.velocity - lastConnectionVelocity) * Time.deltaTime;
        movement -= rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

        float distance = movement.magnitude;

        Quaternion rotation = ball.localRotation;
        if (connectedBody && connectedBody == previousConnectedBody)
        {
            rotation = Quaternion.Euler(connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)) * rotation;
            if (distance < 0.001f)
            {
                ball.localRotation = rotation;
                return;
            }
        }
        else if (distance < 0.001f)
        {
            return;
        }

        float angle = distance * rotationFactor * (180f / Mathf.PI) / ballRadius;
        Vector3 rotationAxis = Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;
        if (ballAlignSpeed > 0f)
        {
            rotation = AlignBallRotation(rotationAxis, rotation, distance);
        }
        ball.localRotation = rotation;
    }

    Quaternion AlignBallRotation(Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
    {
        Vector3 ballAxis = ball.up;
        float dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = ballAlignSpeed * traveledDistance;

        Quaternion newAlignment = Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;
        if (angle <= maxAngle)
        {
            return newAlignment;
        }
        else
        {
            return Quaternion.SlerpUnclamped(rotation, newAlignment, maxAngle / angle);
        }
    }

    bool shouldToogleRoll;
    void FixedUpdate()
    {
        if (isBlocked)
        {
            return;
        }

        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        if (InWater)
        {
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        }

        AdjustVelocity();
        AdjustMaxSpeed();

        if (desiredShoot)
        {
            desiredShoot = false;
            Shoot(gravity);
            shouldToogleRoll = true;
        }

        if (Climbing)
        {
            velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
        }
        else if (InWater)
        {
            velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
        }
        else if (OnGround && velocity.sqrMagnitude < 0.01f)
        {
            velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        }
        else if (desiresClimbing && OnGround)
        {
            velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
        }
        else
        {
            velocity += gravity * Time.deltaTime;
        }

        body.velocity = velocity;

        

        ClearState();
    }

    void ClearState()
    {
        lastContactNormal = contactNormal;
        lastSteepNormal = steepNormal;
        lastConnectionVelocity = connectionVelocity;
        groundContactCount = steepContactCount = climbContactCount = 0;
        contactNormal = steepNormal = climbNormal = Vector3.zero;
        connectionVelocity = Vector3.zero;
        previousConnectedBody = connectedBody;
        connectedBody = null;
        submergence = 0f;
    }

    void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;
        if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;
            if (stepsSinceLastJump > 1)
            {
                shootPhase = 0;
            }
            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }

        if (connectedBody)
        {
            if (connectedBody.isKinematic || connectedBody.mass >= body.mass)
            {
                UpdateConnectionState();
            }
        }
    }

    void UpdateConnectionState()
    {
        if (connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }

    bool CheckClimbing()
    {
        if (Climbing)
        {
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, climbNormal);
                if (upDot >= minGroundDotProduct)
                {
                    climbNormal = lastClimbNormal;
                }
            }
            groundContactCount = 1;
            contactNormal = climbNormal;
            return true;
        }
        return false;
    }

    bool CheckSwimming()
    {
        if (Swimming)
        {
            groundContactCount = 0;
            contactNormal = upAxis;
            return true;
        }
        return false;
    }

    bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater)
        {
            return false;
        }
        float speed = velocity.magnitude;
        if (speed > maxSnapSpeed)
        {
            return false;
        }
        if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore))
        {
            return false;
        }

        float upDot = Vector3.Dot(upAxis, hit.normal);
        if (upDot < GetMinDot(hit.collider.gameObject.layer))
        {
            return false;
        }

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);
        if (dot > 0f)
        {
            velocity = (velocity - hit.normal * dot).normalized * speed;
        }
        connectedBody = hit.rigidbody;
        return true;
    }

    bool CheckSteepContacts()
    {
        if (steepContactCount > 1)
        {
            steepNormal.Normalize();
            float upDot = Vector3.Dot(upAxis, steepNormal);
            if (upDot >= minGroundDotProduct)
            {
                steepContactCount = 0;
                groundContactCount = 1;
                contactNormal = steepNormal;
                return true;
            }
        }
        return false;
    }

    int maxSpeedIndex = 0;
    public void IncreaseMaxSpeed()
    {
        maxSpeedIndex++;
        maxSpeedIndex = Mathf.Clamp(maxSpeedIndex, 0, speedLimits.Count - 1);
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    void LowerMaxSpeed()
    {
        maxSpeedIndex--;
        maxSpeedIndex = Mathf.Clamp(maxSpeedIndex, 0, speedLimits.Count- 1);
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    void ResetMaxSpeed()
    {
        maxSpeedIndex = 0;
        maxSpeed = speedLimits[0];
    }

    void AdjustMaxSpeed()
    {
        if (maxSpeedIndex > 0 && velocity.magnitude < speedLimits[maxSpeedIndex - 1] - 5f)
        {
            LowerMaxSpeed();
        }
    }

    void AdjustVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;
        if (Climbing)
        {
            acceleration = maxClimbAcceleration;
            speed = maxClimbSpeed;
            xAxis = Vector3.Cross(contactNormal, upAxis);
            zAxis = upAxis;
        }
        else if (InWater)
        {
            float swimFactor = Mathf.Min(1f, submergence / swimThreshold);
            acceleration = Mathf.LerpUnclamped(OnGround ? maxAcceleration : maxAirAcceleration,maxSwimAcceleration, swimFactor);
            speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        else
        {
            acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed; // GESTION DE LA VITESSE MAX PAR PALIER ICI !
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ? playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;
        if (Swimming)
        {
            velocity += upAxis * adjustment.y;
        }

        Velocity = velocity.magnitude;
    }

    

    Vector3 shootdirectiondebug;
    void Shoot(Vector3 gravity)
    {
        Vector3 shootDirection;

        if (maxShoots <= 0 || shootPhase >= maxShoots)
        {
            return;
        }

        stepsSinceLastJump = 0;
        shootPhase += 1;
        float shootSpeed = Mathf.Sqrt(2f * gravity.magnitude * shootHeight);
        if (InWater)
        {
            shootSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
        }

        shootDirection = playerInputSpace.forward;
        shootDirection = Quaternion.AngleAxis(shootingAngle, playerInputSpace.right) * shootDirection;
                
        velocity = shootDirection * (shootSpeed * EvaluateShootFactor() + velocity.magnitude);

        IncreaseMaxSpeed();
    }


    //public float minShootFactor, maxShootFactor;
    float EvaluateShootFactor()
    {
        float abs = velocity.magnitude / speedLimits[speedLimits.Count - 1];
        abs *= shootCurve.keys[shootCurve.length - 1].time;

        float result = shootCurve.Evaluate(abs);

        Debug.Log(result);

        return result;
    }

    void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    void EvaluateCollision(Collision collision)
    {
        if (Swimming)
        {
            return;
        }
        int layer = collision.gameObject.layer;
        float minDot = GetMinDot(layer);
        for (int i = 0; i < collision.contactCount; i++)
        {
            Vector3 normal = collision.GetContact(i).normal;
            float upDot = Vector3.Dot(upAxis, normal);
            if (upDot >= minDot)
            {
                groundContactCount += 1;
                contactNormal += normal;
                connectedBody = collision.rigidbody;
            }
            else
            {
                if (upDot > -0.01f)
                {
                    steepContactCount += 1;
                    steepNormal += normal;
                    if (groundContactCount == 0)
                    {
                        connectedBody = collision.rigidbody;
                    }
                }
                if (
                    desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0
                )
                {
                    climbContactCount += 1;
                    climbNormal += normal;
                    lastClimbNormal = normal;
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence(other);
        }
    }

    void OnTriggerStay(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0)
        {
            EvaluateSubmergence(other);
        }
    }

    void EvaluateSubmergence(Collider collider)
    {
        if (Physics.Raycast(body.position + upAxis * submergenceOffset, -upAxis, out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide))
        {
            submergence = 1f - hit.distance / submergenceRange;
        }
        else
        {
            submergence = 1f;
        }
        if (Swimming)
        {
            connectedBody = collider.attachedRigidbody;
        }
    }

    Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    public void UpdatePCData()
    {
        maxAcceleration = PCData.maxAcceleration;
        maxAirAcceleration = PCData.maxAirAcceleration;
        shootHeight = PCData.shootHeight;
        maxShoots = PCData.maxShoots;
        maxGroundAngle = PCData.maxGroundAngle;
        maxSnapSpeed = PCData.maxSnapSpeed;
        probeDistance = PCData.probeDistance;
        speedLimits = PCData.speedLimits;
        speedLimitMargin = PCData.speedLimitMargin;
        rollingMaterial = PCData.rollingMaterial;
        aimingMaterial = PCData.aimingMaterial;
        shootingAngle = PCData.shootingAngle;
        shootCurve = PCData.shootCurve;
    }

    public void SetPCData(PlayerControllerData PCData)
    {
        this.PCData = PCData;
        UpdatePCData();
    }

    public PlayerControllerData GetPCData()
    {
        return PCData;
    }

    public void Teleport(Vector3 position)
    {
        body.position = position;
    }

    public void Teleport(Transform transform)
    {
        body.position = transform.position;
        body.rotation = transform.rotation;
    }

    public void Teleport(Vector3 position, Quaternion rotation)
    {
        body.position = position;
        body.rotation = rotation;
    }

    public Vector3 GetVelocity()
    {
        return body.velocity;
    }


    private Vector3 savedVelocity;
    private bool isBlocked;

    /// <summary>
    /// Arrête la balle en cours de déplacement et sauvegarde sa vélocité. La balle ne réagira plus aux intéractions venants de l'environnement ou du joueur. 
    /// </summary>
    public void Block()
    {
        savedVelocity = body.velocity;
        body.velocity = Vector3.zero;
        isBlocked = true;
    }

    /// <summary>
    /// Débloque la balle. Si le paramètre resetMovement est "true", la balle reprend la trajectoire qu'elle avait avant bloquage, sinon elle aura une vélocité nulle.
    /// </summary>
    /// <param name="resetMovement"></param>
    public void UnBlock(bool resetMovement)
    {
        if (!resetMovement)
        {
            body.velocity = savedVelocity;
            savedVelocity = Vector3.zero;
        }

        isBlocked = false;
    }
}