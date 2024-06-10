using System;
using System.Collections.Generic;
using UnityEngine;

public class PC_MovingSphere : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private AudioSource sfxShoot;
    [SerializeField] private AudioSource sfxSpeed;
    [SerializeField] private Transform ball = default;
    [SerializeField] private GameObject shootingIndicator;

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
    private float Velocity;
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
    private int stepsSinceLastGrounded, stepsSinceLastJump;
    private MeshRenderer meshRenderer;

    private void OnValidate()
    {
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
        minStairsDotProduct = Mathf.Cos(maxStairsAngle * Mathf.Deg2Rad);
        minClimbDotProduct = Mathf.Cos(maxClimbAngle * Mathf.Deg2Rad);

        /*
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
        */
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
        //UnFreeze();
        ResetMaxSpeed();
        playerInputSpace = CameraManager.Instance.GetLookingDirection();
    }

    private bool isAiming;

    private void Update()
    {
        playerInputSpace = CameraManager.Instance.GetLookingDirection();
        UpdatePCData();

        if (isBlocked) return;

        if (shouldToogleRoll)
        {
            shouldToogleRoll = false;
            ToggleRoll(false);
        }
        else
        {
            if (Input.GetMouseButtonDown(1) && Time.timeScale > 0) ToggleAim(); // Activation du mode Aim

            if (isAiming && Input.GetMouseButtonUp(1) && Time.timeScale > 0) ToggleRoll(true); // Desactivation du mode Aim
        }

        if (isAiming) HandleAim();
        else HandleRoll();

        UpdateBall();
    }

    private bool shouldToogleRoll;

    private void FixedUpdate()
    {
        if (isBlocked) return;

        Vector3 gravity = CustomGravity.GetGravity(body.position, out upAxis);
        UpdateState();

        if (InWater) velocity *= 1f - waterDrag * submergence * Time.deltaTime;

        AdjustVelocity();
        AdjustMaxSpeed();

        if (desiredShoot)
        {
            desiredShoot = false;
            Shoot(gravity);
            shouldToogleRoll = true;
        }

        if (Climbing) velocity -= contactNormal * (maxClimbAcceleration * 0.9f * Time.deltaTime);
        else if (InWater) velocity += gravity * ((1f - buoyancy * submergence) * Time.deltaTime);
        else if (OnGround && velocity.sqrMagnitude < 0.01f) velocity += contactNormal * (Vector3.Dot(gravity, contactNormal) * Time.deltaTime);
        else if (desiresClimbing && OnGround) velocity += (gravity - contactNormal * (maxClimbAcceleration * 0.9f)) * Time.deltaTime;
        else velocity += gravity * Time.deltaTime;

        if (!isFreezed) body.velocity = velocity;

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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    public void PreventSnapToGround()
    {
        stepsSinceLastJump = -1;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private void ShowShootingIndicator()
    {
        shootingIndicator.transform.rotation = CameraManager.Instance.GetLookingDirection().rotation;
        //shootingIndicator.transform.rotation = Quaternion.Euler(-shootingAngle, 0, 0) * shootingIndicator.transform.rotation;
        shootingIndicator.SetActive(true);
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private void HideShootingIndicator()
    {
        shootingIndicator.SetActive(false);
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    public void ToggleAim()
    {
        if (!canShoot || shootCharges < 1) return;

        ShowShootingIndicator();
        isAiming = true;

        if (aimingMaterial != null) meshRenderer.material = aimingMaterial;

        CameraManager.Instance.ActivateAimMode();
        shootingIndicator.GetComponent<Animator>().SetBool("Show", true);
        Time.timeScale = 0.1f;
    }

    /// <summary>
    /// Active le mode Roll. Si reset est true, la caméra reprendra la place qu'elle avait lors de la désactivation du mode Roll.
    /// </summary>
    /// <param name="reset"></param>
    public void ToggleRoll(bool reset)
    {
        shootingIndicator.GetComponent<Animator>().SetBool("Show", false);
        HideShootingIndicator();
        Time.timeScale = 1.0f;
        isAiming = false;

        if(rollingMaterial != null) meshRenderer.material = rollingMaterial;

        CameraManager.Instance.ActivateFollowMode(reset);
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private void HandleAim()
    {
        desiredShoot |= Input.GetMouseButtonDown(0);
        shootingIndicator.transform.rotation = Quaternion.Euler(CameraManager.Instance.GetLookingDirection().rotation.eulerAngles.x + shootingAngle / 2, CameraManager.Instance.GetLookingDirection().rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
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

        if (Swimming) desiresClimbing = false;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private void UpdateBall()
    {
        Vector3 rotationPlaneNormal = lastContactNormal;
        float rotationFactor = 1f;

        if (!OnGround)
        {
            if (OnSteep) rotationPlaneNormal = lastSteepNormal;
            else rotationFactor = ballAirRotation;
        }

        if (body.velocity.magnitude > 10 && particles != null && particles.isStopped) particles.Play();
        else if (body.velocity.magnitude <= 10 && particles != null && particles.isPlaying) particles.Stop();

        if (body.velocity.magnitude > 10 && sfxSpeed != null && !sfxSpeed.isPlaying) sfxSpeed.Play();
        else if (body.velocity.magnitude <= 10 && sfxSpeed != null && sfxSpeed.isPlaying) sfxSpeed.Stop();

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
        else if (distance < 0.001f) return;

        float angle = distance * rotationFactor * (180f / Mathf.PI) / ballRadius;
        Vector3 rotationAxis = Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;

        if (ballAlignSpeed > 0f)  rotation = AlignBallRotation(rotationAxis, rotation, distance);

        if (canTurn) ball.localRotation = rotation;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <param name="rotationAxis"></param>
    /// <param name="rotation"></param>
    /// <param name="traveledDistance"></param>
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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastJump += 1;
        velocity = body.velocity;

        if (CheckClimbing() || CheckSwimming() || OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;

            if (stepsSinceLastJump > 1 & shootCharges <= 0)
            {
                shootCharges = 1;

                if (UIManager.instance != null) UIManager.instance.ShootInterface(false);
            }
            
            if (groundContactCount > 1) contactNormal.Normalize();
        }
        else contactNormal = upAxis;

        if (connectedBody && (connectedBody.isKinematic || connectedBody.mass >= body.mass)) UpdateConnectionState();
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
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

    /// <summary>
    /// Retourne si la balle nage
    /// </summary>
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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    private bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1 || stepsSinceLastJump <= 2 || InWater) return false;

        float speed = velocity.magnitude;

        if (speed > maxSnapSpeed) return false;
        
        if (!Physics.Raycast(body.position, -upAxis, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore)) return false;

        float upDot = Vector3.Dot(upAxis, hit.normal);

        if (upDot < GetMinDot(hit.collider.gameObject.layer)) return false;

        groundContactCount = 1;
        contactNormal = hit.normal;
        float dot = Vector3.Dot(velocity, hit.normal);

        if (dot > 0f) velocity = (velocity - hit.normal * dot).normalized * speed;

        connectedBody = hit.rigidbody;

        return true;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
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

    /// <summary>
    /// Diminue la vitesse maximal de la balle
    /// </summary>
    private void LowerMaxSpeed()
    {
        maxSpeedIndex--;
        maxSpeedIndex = Mathf.Clamp(maxSpeedIndex, 0, speedLimits.Count- 1);
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// Réinitialise la vitesse maximal de la balle
    /// </summary>
    private void ResetMaxSpeed()
    {
        maxSpeedIndex = 0;
        maxSpeed =  speedLimits.Count > 0 ? speedLimits[0] : 20f;
    }

    /// <summary>
    /// Ajuste la vitesse maximal de la balle
    /// </summary>
    private void AdjustMaxSpeed()
    {
        if (isVelocityClamped && maxSpeedIndex > 0 && velocity.magnitude < speedLimits[maxSpeedIndex - 1] - speedLimitMargin) LowerMaxSpeed();
    }

    private bool isVelocityClamped = true;

    /// <summary>
    /// Ajuste la vélocité de la balle
    /// </summary>
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

        Velocity = velocity.magnitude;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    public void ClampVelocity()
    {
        isVelocityClamped = true;

        for (int i = 0; i < speedLimits.Count; i++)
        {
            if (velocity.magnitude < speedLimits[i])
            {
                maxSpeedIndex = i;
                maxSpeed = speedLimits[maxSpeedIndex];

                break;
            }
        }

        maxSpeedIndex = speedLimits.Count - 1;
        maxSpeed = speedLimits[maxSpeedIndex];
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    public void UnClampVelocity()
    {
        isVelocityClamped = false;
        IncreaseSpeedLimitToMaximum();
    }

    private float shootingFactor = 0.5f;
    private Vector3 shootdirectiondebug;

    /// <summary>
    /// La fonction tire de la balle
    /// </summary>
    /// <param name="gravity"></param>
    private void Shoot(Vector3 gravity)
    {
        if (!canShoot) return;

        Vector3 shootDirection;

        if (maxShoots <= 0 || shootCharges <= 0) return;

        stepsSinceLastJump = 0;
        shootCharges -= 1;

        if (sfxShoot != null) sfxShoot.Play();

        if (UIManager.instance != null) UIManager.instance.ShootInterface(true);

        float shootSpeed = Mathf.Sqrt(2f * gravity.magnitude * shootHeight);
        
        //if (InWater)
        //{
        //    shootSpeed *= Mathf.Max(0f, 1f - submergence / swimThreshold);
        //}

        shootDirection = playerInputSpace.forward;
        shootDirection = Quaternion.AngleAxis(shootingAngle, playerInputSpace.right) * shootDirection;

        IncreaseSpeedLimit();

        //float shootForce = shootSpeed * EvaluateShootFactor();
        //shootForce = Mathf.Clamp(shootForce, minShootForce, maxShootForce);

        //velocity = shootDirection * (shootForce + velocity.magnitude);

        float localMaxSpeed = Mathf.Max(velocity.magnitude, maxSpeed);

        if (maxSpeedIndex == speedLimits.Count - 1) velocity = shootDirection * localMaxSpeed;
        else velocity = shootDirection * (localMaxSpeed + (speedLimits[maxSpeedIndex + 1] - localMaxSpeed) * shootingFactor);
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <param name="collision"></param>
    private void EvaluateCollision(Collision collision)
    {
        if (Swimming) return;

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

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <param name="collider"></param>
    private void EvaluateSubmergence(Collider collider)
    {
        if (Physics.Raycast(body.position + upAxis * submergenceOffset, -upAxis, out RaycastHit hit, submergenceRange + 1f, waterMask, QueryTriggerInteraction.Collide)) submergence = 1f - hit.distance / submergenceRange;
        else submergence = 1f;

        if (Swimming) connectedBody = collider.attachedRigidbody;
    }

    /// <summary>
    /// Renvoie ??? (A compléter)
    /// </summary>
    /// <param name="direction"></param>
    /// <param name="normal"></param>
    private Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <param name="layer"></param>
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
        body.position = position;
    }

    /// <summary>
    /// Téléporte le joueur à la position et la rotation donnée par transform, en gardant sa vélocité
    /// </summary>
    /// <param name="transform"></param>
    public void Teleport(Transform transform)
    {
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
        body.position = position;
        //body.rotation = rotation;
    }

    /// <summary>
    /// Renvoie le vecteur vélocité du joueur. Utiliser la fonction Vector3.magnitude pour obtenir la vitesse en m/s. Utiliser Vector3.normalized pour obtenir la direction de la vitesse.
    /// </summary>
    /// <returns>Vector3</returns>
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

    private bool canTurn = true;

    /// <summary>
    /// Bloque la direction du joueur, il ne peut plus tourner mais continue de subir la gravité et les frottements
    /// </summary>
    public void FreezeDirection()
    {
        canTurn = false;
    }

    /// <summary>
    /// Débloque le freeze de la direction
    /// </summary>
    public void UnFreezeDirection()
    {
        canTurn = true;
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
    }

    /// <summary>
    /// Ajoute amount charges de tir, dans la limite du max de charges déterminées par le GD
    /// </summary>
    /// <param name="amount"></param>
    public void AddShootCharges(int amount)
    {
        shootCharges = Mathf.Min(maxShoots, shootCharges + 1);

        if (UIManager.instance != null) UIManager.instance.ShootInterface(false);
    }

    /// <summary>
    /// Change la direction de déplacement du player
    /// </summary>
    /// <param name="direction"></param>
    public void SetDirection(Vector3 direction)
    {
        body.velocity = direction.normalized * body.velocity.magnitude;
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
        body.velocity = body.velocity.normalized * speedLimits[maxSpeedIndex];
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
