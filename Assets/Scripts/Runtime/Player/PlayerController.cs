using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private ParticleSystem particlesTrail, particlesShoot;

    [SerializeField] private AudioSource sfxShoot;
    [SerializeField] private AudioSource sfxSpeed;
    [SerializeField] private AudioSource sfxLanding;

    [SerializeField] private Transform ball = default;
    //[SerializeField] private GameObject shootingIndicator;

    // ----------
    // -- Tool --
    // ----------

    [Header("Player Controller")]
    public PlayerControllerData PCData;
    public float maxSpeed;

    #region TOOL PARAMETERS

    private float maxAcceleration, maxAirAcceleration;

    private float shootHeight;
    private int maxShoots;
    private float maxGroundAngle;
    private float maxSnapSpeed;
    private float probeDistance;
    private float speedLimitMargin;
    private Material rollingMaterial, aimingMaterial;
    private float shootingAngle;

    public AnimationCurve rotationCurve;

    private List<float> speedLimits;

    #endregion

    private Transform playerInputSpace;

    private float maxClimbSpeed = 4f, maxSwimSpeed = 5f;

    private float maxClimbAcceleration = 40f, maxSwimAcceleration = 5f;

    private float maxStairsAngle = 50f; // Non utilisé
    private float maxClimbAngle = 140f; // Non utilisé
    private float submergenceOffset = 0.5f;
    private float submergenceRange = 1f;
    private float buoyancy = 1f;
    private float waterDrag = 1f;
    private float swimThreshold = 0.5f;

    private LayerMask probeMask = -1;

    private LayerMask stairsMask = -1, climbMask = -1, waterMask = 0;

    private float ballRadius = 0.5f;

    private float ballAlignSpeed = 180f;

    private float ballAirRotation = 0.5f;

    private Rigidbody body, connectedBody, previousConnectedBody;
    private Vector3 playerInput;
    private Vector3 velocity, connectionVelocity;
    private Vector3 connectionWorldPosition, connectionLocalPosition;
    private Vector3 upAxis, rightAxis, forwardAxis;
    private bool desiredShoot, desiresClimbing;
    private Vector3 contactNormal, steepNormal, climbNormal, lastClimbNormal;
    private Vector3 lastContactNormal, lastSteepNormal, lastConnectionVelocity;
    private int groundContactCount, steepContactCount, climbContactCount;
    private bool OnGround => groundContactCount > 0;
    private bool OnSteep => steepContactCount > 0;
    private bool Climbing => false; // climbContactCount > 0 && stepsSinceLastJump > 2;
    private bool InWater => false; // submergence > 0f;
    private bool Swimming => false; // submergence >= swimThreshold;
    private float submergence;
    private int shootCharges;

    private float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    private int stepsSinceLastGrounded, stepsSinceLastShoot;
    private MeshRenderer meshRenderer;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);
    }

    private void Awake()
    {
        body = GetComponent<Rigidbody>();
        body.useGravity = false;
        meshRenderer = ball.GetComponent<MeshRenderer>();
        OnValidate();
    }

    private void Start()
    {
        UpdatePCData();
        ResetMaxSpeed();
        playerInputSpace = cameraManager.GetLookingDirection();
    }

    private bool isAiming;
    bool canAim = true;
    private void Update()
    {
        playerInputSpace = cameraManager.GetLookingDirection();
        UpdatePCData();

        if (isBlocked) return;

        if (shouldToogleRoll)
        {
            shouldToogleRoll = false;
            ToggleRoll(false);
        }
        else
        {
            if ((Input.GetButtonDown("Aim") || Input.GetAxisRaw("Aim GamePad") == 1) && Time.timeScale > 0 && !isAiming && canAim)
            {
                ToggleAim(); // Activation du mode Aim

                canAim = false;
            }
            else if((Input.GetButtonUp("Aim") || (Input.GetAxisRaw("Aim GamePad") == 0) && !Input.GetButton("Aim")) && Time.timeScale > 0 && isAiming)
            {
                ToggleRoll(true); // Desactivation du mode Aim
                canAim = true;
            }
        }

        if ((Input.GetButtonUp("Aim") || (Input.GetAxisRaw("Aim GamePad") == 0))){
            canAim = true;
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

    private bool shouldToogleRoll;
    bool desiredJump;

    private void FixedUpdate()
    {
        if (isBlocked)
        {
            return;
        }

        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        AdjustVelocity();
        AdjustMaxSpeed();

        if (desiredJump)
        {
            desiredJump = false;
            Jump(gravity);
        }

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
        else if (OnGround && velocity.sqrMagnitude < 0.001f)
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

        if (!isFreezed)
        {
            body.linearVelocity = velocity;
        }

        ClearState();
    }

    private void OnCollisionEnter(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnCollisionStay(Collision collision)
    {
        EvaluateCollision(collision);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0) EvaluateSubmergence(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if ((waterMask & (1 << other.gameObject.layer)) != 0) EvaluateSubmergence(other);
    }


    public void PreventSnapToGround()
    {
        stepsSinceLastShoot = -1;
    }


    public void ToggleAim()
    {
        if (!canShoot || shootCharges < 1)
        {
            return;
        }

        isAiming = true;

        if (aimingMaterial != null) meshRenderer.material = aimingMaterial;

        cameraManager.ActivateAimMode();
        Time.timeScale = 0.1f;
    }

    /// <summary>
    /// Active le mode Roll. Si reset est true, la caméra reprendra la place qu'elle avait lors de la désactivation du mode Roll.
    /// </summary>
    /// <param name="reset"></param>
    public void ToggleRoll(bool reset)
    {
        Time.timeScale = 1.0f;
        isAiming = false;

        if (rollingMaterial != null)
        {
            meshRenderer.material = rollingMaterial;
        }

        if (shootCharges > 0)
        {
            meshRenderer.material.SetColor("_BaseColor", Color.white);
        }
        else
        {
            meshRenderer.material.SetColor("_BaseColor", Color.grey);
        }

        cameraManager.ActivateFollowMode(reset);
    }


    private void HandleAim()
    {
        desiredShoot |= Input.GetButtonDown("Shoot") || Input.GetAxisRaw("Shoot GamePad") == 1;
    }


    private void HandleRoll()
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

        desiredJump |= Input.GetButtonDown("Jump");
    }

    private void UpdateBall()
    {
        Vector3 rotationPlaneNormal = lastContactNormal;
        float rotationFactor = 1f;

        if (!OnGround)
        {
            if (OnSteep) rotationPlaneNormal = lastSteepNormal;
            else rotationFactor = ballAirRotation;
        }

        if (body.linearVelocity.magnitude > 10 && particlesTrail != null && particlesTrail.isStopped)
        {
            particlesTrail.Play();
        }
        else if (body.linearVelocity.magnitude <= 10 && particlesTrail != null && particlesTrail.isPlaying)
        {
            particlesTrail.Stop();
        }

        if (body.linearVelocity.magnitude > 10 && sfxSpeed != null && !sfxSpeed.isPlaying)
        {
            sfxSpeed.Play();
        }
        else if (body.linearVelocity.magnitude <= 10 && sfxSpeed != null && sfxSpeed.isPlaying)
        {
            sfxSpeed.Stop();
        }

        Vector3 movement = (body.linearVelocity - lastConnectionVelocity) * Time.deltaTime;
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
        else if (distance < 0.001f) return;

        float angle = distance * rotationFactor * (180f / Mathf.PI) / ballRadius;
        Vector3 rotationAxis = Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;

        if (ballAlignSpeed > 0f)  rotation = AlignBallRotation(rotationAxis, rotation, distance);

        if (canTurn) ball.localRotation = rotation;
    }


    private Quaternion AlignBallRotation(Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
    {
        Vector3 ballAxis = ball.up;
        float dot = Mathf.Clamp(Vector3.Dot(ballAxis, rotationAxis), -1f, 1f);
        float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;
        float maxAngle = ballAlignSpeed * traveledDistance;

        Quaternion newAlignment = Quaternion.FromToRotation(ballAxis, rotationAxis) * rotation;

        if (angle <= maxAngle) return newAlignment;
        else return Quaternion.SlerpUnclamped(rotation, newAlignment, maxAngle / angle);
    }


    private void ClearState()
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



    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastShoot += 1;
        velocity = body.linearVelocity;

        if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;

            if (stepsSinceLastShoot > 1 & shootCharges <= 0)
            {
                StartCoroutine(cameraManager.LandingShake());
                StartCoroutine(Rumble(0.2f, 0.4f, 0.4f));

                if (sfxLanding != null)
                {
                    sfxLanding.Play();
                }

                shootCharges = 1;

                meshRenderer.material.SetColor("_BaseColor", Color.white);

                if (UIManager.instance != null)
                {
                    UIManager.instance.ShootInterface(false);
                }
            }

            jumpPhase = 0;

            cameraManager.Shake(0);

            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            cameraManager.Shake(body.linearVelocity.magnitude / GetMaxSpeedLimit());
            contactNormal = upAxis;
        }

        if (connectedBody && (connectedBody.isKinematic || connectedBody.mass >= body.mass))
        {
            UpdateConnectionState();
        }
    }


    private void UpdateConnectionState()
    {
        if (connectedBody == previousConnectedBody)
        {
            Vector3 connectionMovement = connectedBody.transform.TransformPoint(connectionLocalPosition) - connectionWorldPosition;
            connectionVelocity = connectionMovement / Time.deltaTime;
        }

        connectionWorldPosition = body.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }


    private bool CheckClimbing()
    {
        if (Climbing)
        {
            if (climbContactCount > 1)
            {
                climbNormal.Normalize();
                float upDot = Vector3.Dot(upAxis, climbNormal);
                if (upDot >= minGroundDotProduct) climbNormal = lastClimbNormal;
            }

            groundContactCount = 1;
            contactNormal = climbNormal;

            return true;
        }

        return false;
    }


    private bool CheckSwimming()
    {
        if (Swimming)
        {
            groundContactCount = 0;
            contactNormal = upAxis;

            return true;
        }

        return false;
    }


    private bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastShoot <= 2 || InWater)
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


    private bool CheckSteepContacts()
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

    private int maxSpeedIndex = 0;


    private void LowerMaxSpeed()
    {
        maxSpeedIndex--;
        maxSpeedIndex = Mathf.Clamp(maxSpeedIndex, 0, speedLimits.Count- 1);
        maxSpeed = speedLimits[maxSpeedIndex];
    }


    private void ResetMaxSpeed()
    {
        maxSpeedIndex = 0;
        maxSpeed =  speedLimits.Count > 0 ? speedLimits[0] : 20f;
    }


    private void AdjustMaxSpeed()
    {
        if (isVelocityClamped && maxSpeedIndex > 0 && velocity.magnitude < speedLimits[maxSpeedIndex - 1] - speedLimitMargin)
        {
            LowerMaxSpeed();
        }

        if (!isVelocityClamped)
        {
            for (int i = 0; i < speedLimits.Count; i++)
            {
                if (velocity.magnitude < speedLimits[i])
                {
                    maxSpeedIndex = i;
                    maxSpeed = speedLimits[maxSpeedIndex];

                    break;
                }
            }

            // maxSpeedIndex = speedLimits.Count - 1;
            maxSpeed = speedLimits[maxSpeedIndex];
        }
    }

    private bool isVelocityClamped = true;


    private void AdjustVelocity()
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

        float turningFactor = rotationCurve.Evaluate(velocity.magnitude / speedLimits[speedLimits.Count - 1]);
        velocity += xAxis * (adjustment.x * turningFactor) + zAxis * (adjustment.z * 1);

        if (Swimming) velocity += upAxis * adjustment.y;
    }


    public void ClampVelocity()
    {
        isVelocityClamped = true;
    }

    public void UnClampVelocity()
    {
        isVelocityClamped = false;
    }

    public IEnumerator Rumble(float lowFrequencyIntensity, float highFrequencyIntensity, float duration)
    {
        if (Gamepad.current == null)
        {
            yield break;
        }

        Gamepad.current.SetMotorSpeeds(lowFrequencyIntensity, highFrequencyIntensity);

        yield return new WaitForSeconds(duration);

        Gamepad.current.SetMotorSpeeds(0, 0);
    }

    int jumpPhase;
    public int jumpHeight = 2, maxAirJumps = 1;
    void Jump(Vector3 gravity)
    {
        Vector3 jumpDirection;

        if (OnSteep)
        {
            jumpDirection = steepNormal;
            jumpPhase = 0;
        }
        else if (OnGround)
        {
            jumpDirection = contactNormal;
            jumpPhase = 0;
        }
        else if (maxAirJumps > 0 && jumpPhase < maxAirJumps)
        {
            jumpDirection = contactNormal;
        }
        else
        {
            return;
        }

        stepsSinceLastShoot = 0;
        jumpPhase += 1;

        float jumpSpeed = Mathf.Sqrt(2f * gravity.magnitude * jumpHeight);

        jumpDirection = (jumpDirection + upAxis).normalized;
        float alignedSpeed = Vector3.Dot(velocity, jumpDirection);

        if (alignedSpeed > 0f)
        {
            jumpSpeed = Mathf.Max(jumpSpeed - alignedSpeed, 0f);
        }

        velocity += jumpDirection * jumpSpeed;

        StartCoroutine(Rumble(0.2f, 0.2f, 0.2f));
    }

    private float shootingFactor = 0.5f;
    private void Shoot(Vector3 gravity)
    {
        if (!canShoot)
        { 
            return;
        }

        Vector3 shootDirection;

        if (maxShoots <= 0 || shootCharges <= 0)
        {
            return;
        }

        stepsSinceLastShoot = 0;
        shootCharges -= 1;

        meshRenderer.material.SetColor("_BaseColor", Color.grey);

        shootDirection = playerInputSpace.forward;

        if (OnGround && Vector3.Dot(contactNormal, shootDirection) < 0)
        {
            shootDirection = Vector3.ProjectOnPlane(shootDirection, contactNormal).normalized;
        }

        shootDirection = Quaternion.AngleAxis(shootingAngle, playerInputSpace.right) * shootDirection;

        IncreaseSpeedLimit();

        float localMaxSpeed = Mathf.Max(velocity.magnitude, maxSpeed);

        if (maxSpeedIndex == speedLimits.Count - 1)
        {
            velocity = shootDirection * localMaxSpeed;
        }
        else
        {
            velocity = shootDirection * (localMaxSpeed + (speedLimits[maxSpeedIndex + 1] - localMaxSpeed) * shootingFactor);
        }

        if (sfxShoot != null)
        {
            sfxShoot.Play();
        }

        if (UIManager.instance != null)
        {
            UIManager.instance.ShootInterface(true);
        }

        if (particlesShoot)
        {
            particlesShoot.Play();
        }

        StartCoroutine(cameraManager.BoostEffect());
        StartCoroutine(Rumble(0.4f, 0.2f, 0.4f));
    }


    private void EvaluateCollision(Collision collision)
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

                    if (groundContactCount == 0) connectedBody = collision.rigidbody;
                }

                if (desiresClimbing && upDot >= minClimbDotProduct && (climbMask & (1 << layer)) != 0)
                {
                    climbContactCount += 1;
                    climbNormal += normal;
                    lastClimbNormal = normal;
                    connectedBody = collision.rigidbody;
                }
            }
        }
    }


    private void EvaluateSubmergence(Collider collider)
    {
        if (Physics.Raycast(body.position + upAxis * submergenceOffset, -upAxis, out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide)) submergence = 1f - hit.distance / submergenceRange;
        else submergence = 1f;

        if (Swimming) connectedBody = collider.attachedRigidbody;
    }


    private Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }


    private float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }

    /// <summary>
    /// Met à jour les données du player controller
    /// </summary>
    public void UpdatePCData()
    {
        maxAcceleration = PCData.maxAcceleration;
        maxAirAcceleration = PCData.maxAirAcceleration;
        shootHeight = PCData.shootHeight;
        maxShoots = PCData.maxShoots;
        shootingFactor = PCData.shootingFactor;
        maxGroundAngle = PCData.maxGroundAngle;
        maxSnapSpeed = PCData.maxSnapSpeed;
        probeDistance = PCData.probeDistance;
        speedLimits = PCData.speedLimits;
        speedLimitMargin = PCData.speedLimitMargin;
        rollingMaterial = PCData.rollingMaterial;
        aimingMaterial = PCData.aimingMaterial;
        shootingAngle = PCData.shootingAngle;
        rotationCurve = PCData.rotationCurve;
    }

    /// <summary>
    /// Définit les données du player controller
    /// </summary>
    /// <param name="PCData"></param>
    public void SetPCData(PlayerControllerData PCData)
    {
        this.PCData = PCData;
        UpdatePCData();
    }

    /// <summary>
    /// Renvoie les données du player controller
    /// </summary>
    public PlayerControllerData GetPCData()
    {
        return PCData;
    }

    /// <summary>
    /// Téléporte le joueur à la position donnée, en gardant son orientation et sa vélocité
    /// </summary>
    /// <param name="position"></param>
    public void Teleport(Vector3 position)
    {
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
        cameraManager.ResetShake();
        body.position = position;
    }

    /// <summary>
    /// Téléporte le joueur à la position et la rotation donnée par transform, en gardant sa vélocité
    /// </summary>
    /// <param name="transform"></param>
    public void Teleport(Transform transform)
    {
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
        cameraManager.ResetShake();
        body.position = transform.position;
        //body.rotation = transform.rotation;
    }

    /// <summary>
    /// Téléporte le joueur à la position et la rotation données, en gardant sa vélocité
    /// </summary>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    public void Teleport(Vector3 position, Quaternion rotation)
    {
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
        cameraManager.ResetShake();
        body.position = position;
        //body.rotation = rotation;
    }

    /// <summary>
    /// Renvoie le vecteur vélocité du joueur. Utiliser la fonction Vector3.magnitude pour obtenir la vitesse en m/s. Utiliser Vector3.normalized pour obtenir la direction de la vitesse.
    /// </summary>
    /// <returns>Vector3</returns>
    public Vector3 GetVelocity()
    {
        return body.linearVelocity;
    }

    private Vector3 savedVelocity;
    private bool isBlocked;

    /// <summary>
    /// Arrête la balle en cours de déplacement et sauvegarde sa vélocité. La balle ne réagira plus aux intéractions venants de l'environnement ou du joueur. 
    /// </summary>
    public void Block()
    {
        savedVelocity = body.linearVelocity;
        body.linearVelocity = Vector3.zero;
        isBlocked = true;
        cameraManager.ResetShake();
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    /// <summary>
    /// Débloque la balle. Si le paramètre resetMovement est "true", la balle reprend la trajectoire qu'elle avait avant bloquage, sinon elle aura une vélocité nulle.
    /// </summary>
    /// <param name="resetMovement"></param>
    public void UnBlock(bool resetMovement)
    {
        if (!resetMovement)
        {
            body.linearVelocity = savedVelocity;
            savedVelocity = Vector3.zero;
        }

        isBlocked = false;
        cameraManager.ResetShake();
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }

    }

    private bool canTurn = true;

    /// <summary>
    /// Bloque la direction du joueur, il ne peut plus tourner mais continue de subir la gravité et les frottements
    /// </summary>
    public void FreezeDirection()
    {
        canTurn = false;
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    /// <summary>
    /// Débloque le freeze de la direction
    /// </summary>
    public void UnFreezeDirection()
    {
        canTurn = true;
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    private bool isFreezed = false;

    /// <summary>
    /// Bloque la vélocité du joueur, il avance tout droit à vitesse constante sans pouvoir tourner. Peut être désactivé avec la fonction UnFreeze().
    /// </summary>
    public void Freeze()
    {
        FreezeDirection();
        isFreezed = true;
    }

    /// <summary>
    /// Débloque le freeze
    /// </summary>
    public void UnFreeze()
    {
        UnFreezeDirection();
        isFreezed = false;
        StopAllCoroutines();
        if (Gamepad.current != null)
        {
            Gamepad.current.SetMotorSpeeds(0, 0);
        }
    }

    /// <summary>
    /// Ajoute amount charges de tir, dans la limite du max de charges déterminées par le GD
    /// </summary>
    /// <param name="amount"></param>
    public void AddShootCharges(int amount)
    {
        shootCharges = Mathf.Min(maxShoots, shootCharges + 1);

        if (UIManager.instance != null)
        {
            UIManager.instance.ShootInterface(false);
        }
    }

    /// <summary>
    /// Change la direction de déplacement du player
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Vector3 direction)
    {
        body.linearVelocity = direction.normalized * body.linearVelocity.magnitude;
    }

    /// <summary>
    /// Augmente la limite de vitesse actuelle au prochain palier
    /// </summary>
    public void IncreaseSpeedLimit()
    {
        maxSpeedIndex = Mathf.Min(maxSpeedIndex + 1, speedLimits.Count - 1);
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// Augmente la limite de vitesse au palier maximum
    /// </summary>
    public void IncreaseSpeedLimitToMaximum()
    {
        maxSpeedIndex = speedLimits.Count - 1;
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// Augmente la vitesse du player à la vitesse max actuelle
    /// </summary>
    public void IncreaseVelocityToCurrentSpeedLimit()
    {
        body.linearVelocity = body.linearVelocity.normalized * speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// Renvoi la vitesse maximum actuelle
    /// </summary>
    /// <returns></returns>
    public float GetCurrentSpeedLimit()
    {
        return speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// Renvoi la limite de vitesse maximale
    /// </summary>
    /// <returns></returns>
    public float GetMaxSpeedLimit()
    {
        return speedLimits[speedLimits.Count - 1];
    }

    /// <summary>
    /// Renvoi l'index de la limite de vitesse actuelle
    /// </summary>
    /// <returns></returns>
    public int GetSpeedLimitIndex()
    {
        return maxSpeedIndex;
    }

    /// <summary>
    /// Mets la limite de vitesse au palier indiqué par limitIndex
    /// </summary>
    /// <param name="limitIndex"></param>
    public void SetSpeedLimit(int limitIndex)
    {
        if (limitIndex < 0 || limitIndex >= speedLimits.Count) throw new Exception("SpeedLimitIndex out of bounds");

        maxSpeedIndex = limitIndex;
        maxSpeed = speedLimits[maxSpeedIndex];
    }
    
    private bool canShoot = true;

    /// <summary>
    /// Empêche le joueur de tirer. Peut être réactivé avec la fonction ActivateShoot().
    /// </summary>
    public void DeactivateShoot()
    {
        canShoot = false;
    }

    /// <summary>
    /// Autorise le joueur à tirer.
    /// </summary>
    public void ActivateShoot()
    {
        canShoot = true;
    }
}
