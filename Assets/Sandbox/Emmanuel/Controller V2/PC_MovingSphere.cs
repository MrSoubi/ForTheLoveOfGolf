using DG.Tweening;
using UnityEngine;

public class PC_MovingSphere : MonoBehaviour
{
    [HideInInspector] public float maxSpeed = 10f;
    [HideInInspector] public float maxAcceleration = 10f; 
    [HideInInspector] public float maxAirAcceleration = 1f;
    [HideInInspector] public float maxGroundAngle = 25f;
    [HideInInspector] public float maxSnapSpeed = 100f;

    [HideInInspector] public float probeDistance = 1f;

    [HideInInspector] public LayerMask probeMask = -1;

    [HideInInspector] public Material normalMaterial = default;

    [HideInInspector] public float ballRadius = 0.5f;
    [HideInInspector] public float ballAlignSpeed = 180f;
    [HideInInspector] public float ballAirRotation = 0.5f;

    [SerializeField]
    [Tooltip("Utiliser la cam�ra suivant la balle")]
    Transform playerInputSpace = default;

    [SerializeField]
    [Tooltip("Mettre l'enfant Ball")]
    Transform ball = default;
  
    float maxClimbSpeed = 4f, maxSwimSpeed = 5f;

    float
        maxClimbAcceleration = 40f,
        maxSwimAcceleration = 5f;

    float shootHeight = 2f;

    int maxAirShoots = 0;

    [Tooltip("Angle maximum du sol au del� duquel la balle ne prendre plus d'acc�l�ration")]
   
    
    float maxStairsAngle = 50f; // Non utilis�

    float maxClimbAngle = 140f; // Non utilis�

    float submergenceOffset = 0.5f;


    float submergenceRange = 1f;


    float buoyancy = 1f;


    float waterDrag = 1f;


    float swimThreshold = 0.5f;
    
    LayerMask stairsMask = -1, climbMask = -1, waterMask = 0;
        
    Material 
        climbingMaterial = default,
        swimmingMaterial = default;

    [Tooltip("")]
    
        
    float ballSwimRotation = 2f;

    Rigidbody body, connectedBody, previousConnectedBody;

    Vector3 playerInput;

    Vector3 velocity, connectionVelocity;

    Vector3 connectionWorldPosition, connectionLocalPosition;

    Vector3 upAxis, rightAxis, forwardAxis;

    bool desiredJump, desiresClimbing;

    Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;

    Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;

    int groundContactCount, steepContactCount, climbContactCount;

    bool OnGround => groundContactCount > 0;

    bool OnSteep => steepContactCount > 0;

    bool Climbing => false; // climbContactCount > 0 && stepsSinceLastJump > 2;

    bool InWater => false; // submergence > 0f;

    bool Swimming => false; // submergence >= swimThreshold;

    float submergence;


    bool isJumpAllowed = false;

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
    }

    void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = ball.GetComponent<MeshRenderer>();
        OnValidate();
    }

    bool isAiming;
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isAiming = true;
            playerInputSpace.GetComponent<PC_OrbitCamera>().ToggleAimMode();
            Time.timeScale = 0.1f;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Time.timeScale = 1.0f;
            isAiming = false;
            playerInputSpace.GetComponent<PC_OrbitCamera>().ToggleFollowMode();
        }

        if (isAiming)
        {
            HandleAim();
        }
        else
        {
            HandleRoll();
        }
    }

    void HandleAim()
    {
        
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
            forwardAxis =
                ProjectDirectionOnPlane(playerInputSpace.forward, upAxis);
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
        else
        {
            desiredJump |= Input.GetButtonDown("Jump");
            //desiresClimbing = Input.GetButton("Climb");
        }

        UpdateBall();
    }

    void UpdateBall()
    {
        Material ballMaterial = normalMaterial;
        Vector3 rotationPlaneNormal = lastContactNormal;
        float rotationFactor = 1f;
        if (Climbing)
        {
            ballMaterial = climbingMaterial;
        }
        else if (Swimming)
        {
            ballMaterial = swimmingMaterial;
            rotationFactor = ballSwimRotation;
        }
        else if (!OnGround)
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
        meshRenderer.material = ballMaterial;

        Vector3 movement =
            (body.velocity - lastConnectionVelocity) * Time.deltaTime;
        movement -=
            rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

        float distance = movement.magnitude;

        Quaternion rotation = ball.localRotation;
        if (connectedBody && connectedBody == previousConnectedBody)
        {
            rotation = Quaternion.Euler(
                connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)
            ) * rotation;
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
        Vector3 rotationAxis =
            Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;
        if (ballAlignSpeed > 0f)
        {
            rotation = AlignBallRotation(rotationAxis, rotation, distance);
        }
        ball.localRotation = rotation;
    }

    Quaternion AlignBallRotation(
        Vector3 rotationAxis, Quaternion rotation, float traveledDistance
    )
    {
        Vector3 ballAxis = ball.up;
        float dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = ballAlignSpeed * traveledDistance;

        Quaternion newAlignment =
            Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;
        if (angle <= maxAngle)
        {
            return newAlignment;
        }
        else
        {
            return Quaternion.SlerpUnclamped(
                rotation, newAlignment, maxAngle / angle
            );
        }
    }

    void FixedUpdate()
    {
        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        if (InWater)
        {
            velocity *= 1f - waterDrag * submergence * Time.deltaTime;
        }

        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Shoot(gravity);
        }

        if (Climbing)
        {
            velocity -=
                contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
        }
        else if (InWater)
        {
            velocity +=
                gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
        }
        else if (OnGround && velocity.sqrMagnitude < 0.01f)
        {
            velocity +=
                contactNormal *
                (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        }
        else if (desiresClimbing && OnGround)
        {
            velocity +=
                (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) *
                Time.deltaTime;
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
        if (
            CheckClimbing() || CheckSwimming() ||
            OnGround || SnapToGround() || CheckSteepContacts()
        )
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
            Vector3 connectionMovement =
                connectedBody.transform.TransformPoint(connectionLocalPosition) -
                connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }
        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(
            connectionWorldPosition
        );
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
        if (!Physics.Raycast(
            body.position, -upAxis, out RaycastHit hit,
            probeDistance, probeMask, QueryTriggerInteraction.Ignore
        ))
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
            acceleration = Mathf.LerpUnclamped(
                OnGround ? maxAcceleration : maxAirAcceleration,
                maxSwimAcceleration, swimFactor
            );
            speed = Mathf.LerpUnclamped(maxSpeed, maxSwimSpeed, swimFactor);
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        else
        {
            acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
            speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed;
            xAxis = rightAxis;
            zAxis = forwardAxis;
        }
        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x =
            playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z =
            playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = Swimming ?
            playerInput.y * speed - Vector3.Dot(relativeVelocity, upAxis) : 0f;

        adjustment =
            Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        velocity += xAxis * adjustment.x + zAxis * adjustment.z;
        if (Swimming)
        {
            velocity += upAxis * adjustment.y;
        }
    }

    void Shoot(Vector3 gravity)
    {
        Vector3 shootDirection;
        if (OnGround)
        {
            shootDirection = contactNormal;
        }
        else if (OnSteep)
        {
            shootDirection = steepNormal;
            shootPhase = 0;
        }
        else if (maxAirShoots > 0 && shootPhase <= maxAirShoots)
        {
            if (shootPhase == 0)
            {
                shootPhase = 1;
            }
            shootDirection = contactNormal;
        }
        else
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
        shootDirection = (shootDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, shootDirection);
        if (alignedSpeed > 0f)
        {
            shootSpeed = Mathf.Max(shootSpeed - alignedSpeed, 0f);
        }
        velocity += shootDirection * shootSpeed;
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
                    desiresClimbing && upDot >= minClimbDotProduct &&
                    (climbMask & (1 << layer)) != 0
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
        if (Physics.Raycast(
            body.position + upAxis * submergenceOffset,
            -upAxis, out RaycastHit hit, submergenceRange + 1f,
            waterMask, QueryTriggerInteraction.Collide
        ))
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
        return (stairsMask & (1 << layer)) == 0 ?
            minGroundDotProduct : minStairsDotProduct;
    }
}
