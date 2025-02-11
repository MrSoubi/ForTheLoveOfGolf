using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Rigidbody rb;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Transform playerInputSpace;

    [Header("Player Controller")]
    [SerializeField] float maxSpeed;
    [SerializeField] float maxAcceleration, maxAirAcceleration;
    [SerializeField] float shootHeight;
    [SerializeField] int maxShoots;
    [SerializeField] float maxGroundAngle;
    [SerializeField] float maxSnapSpeed;
    [SerializeField] float probeDistance;
    [SerializeField] float speedLimitMargin;
    [SerializeField] Material rollingMaterial, aimingMaterial;
    [SerializeField] float shootingAngle;

    public AnimationCurve rotationCurve;

    private float maxClimbSpeed = 4f;

    private float maxClimbAcceleration = 40f;


    [SerializeField] LayerMask probeMask = -1;

    [SerializeField] LayerMask stairsMask = -1, climbMask = -1, waterMask = 0;

    private float ballRadius = 0.5f;

    private float ballAlignSpeed = 180f;

    private float ballAirRotation = 0.5f;

    private Rigidbody connectedBody, previousConnectedBody;
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

    private float minGroundDotProduct, minStairsDotProduct, minClimbDotProduct;
    private int stepsSinceLastGrounded, stepsSinceLastShoot;

    private void Awake()
    {
        rb.useGravity = false;
        minGroundDotProduct = Mathf.Cos(maxGroundAngle * Mathf.Deg2Rad);
    }

    private bool isAiming;
    //bool canAim = true;

    private void Update()
    {
        HandleRoll();

        UpdateBall();
    }

    //private bool shouldToogleRoll;
    bool desiredJump;

    private void FixedUpdate()
    {
        Vector3 gravity = CustomGravity.GetGravity(rb.position, out upAxis);

        UpdateState();

        AdjustVelocity();

        if (desiredJump)
        {
            desiredJump = false;
            Jump(gravity);
        }

        if (OnGround && velocity.sqrMagnitude < 0.001f)
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

    public void PreventSnapToGround()
    {
        stepsSinceLastShoot = -1;
    }


    private void HandleRoll()
    {
        playerInput.x = Input.GetAxis("Horizontal");
        playerInput.z = Input.GetAxis("Vertical");
        playerInput.y = 0f;
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

        Vector3 movement = (rb.linearVelocity - lastConnectionVelocity) * Time.deltaTime;
        movement -= rotationPlaneNormal * Vector3.Dot(movement, rotationPlaneNormal);

        float distance = movement.magnitude;

        Quaternion rotation = transform.localRotation;
        if (connectedBody && connectedBody == previousConnectedBody)
        {
            rotation = Quaternion.Euler(connectedBody.angularVelocity * (Mathf.Rad2Deg * Time.deltaTime)) * rotation;

            if (distance < 0.001f)
            {
                transform.localRotation = rotation;
                
                return;
            }
        }
        else if (distance < 0.001f) return;

        float angle = distance * rotationFactor * (180f / Mathf.PI) / ballRadius;
        Vector3 rotationAxis = Vector3.Cross(rotationPlaneNormal, movement).normalized;
        rotation = Quaternion.Euler(rotationAxis * angle) * rotation;

        if (ballAlignSpeed > 0f)  rotation = AlignBallRotation(rotationAxis, rotation, distance);
    }


    private Quaternion AlignBallRotation(Vector3 rotationAxis, Quaternion rotation, float traveledDistance)
    {
        Vector3 ballAxis = transform.up;
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
    }



    private void UpdateState()
    {
        stepsSinceLastGrounded += 1;
        stepsSinceLastShoot += 1;

        velocity = rb.linearVelocity;

        if (OnGround || SnapToGround() || CheckSteepContacts())
        {
            stepsSinceLastGrounded = 0;

            jumpPhase = 0;

            if (groundContactCount > 1)
            {
                contactNormal.Normalize();
            }
        }
        else
        {
            contactNormal = upAxis;
        }

        if (connectedBody && (connectedBody.isKinematic || connectedBody.mass >= rb.mass))
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

        connectionWorldPosition = rb.position;
        connectionLocalPosition = connectedBody.transform.InverseTransformPoint(connectionWorldPosition);
    }


    private bool SnapToGround()
    {
        if (stepsSinceLastGrounded > 1)
        {
            return false;
        }

        float speed = velocity.magnitude;

        if (speed > maxSnapSpeed)
        {
            return false;
        }

        if (!Physics.Raycast(rb.position, -upAxis, out RaycastHit hit, probeDistance, probeMask, QueryTriggerInteraction.Ignore))
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

    private void AdjustVelocity()
    {
        float acceleration, speed;
        Vector3 xAxis, zAxis;


        acceleration = OnGround ? maxAcceleration : maxAirAcceleration;
        speed = OnGround && desiresClimbing ? maxClimbSpeed : maxSpeed; // GESTION DE LA VITESSE MAX PAR PALIER ICI !
        xAxis = rightAxis;
        zAxis = forwardAxis;


        xAxis = ProjectDirectionOnPlane(xAxis, contactNormal);
        zAxis = ProjectDirectionOnPlane(zAxis, contactNormal);

        Vector3 relativeVelocity = velocity - connectionVelocity;

        Vector3 adjustment;
        adjustment.x = playerInput.x * speed - Vector3.Dot(relativeVelocity, xAxis);
        adjustment.z = playerInput.z * speed - Vector3.Dot(relativeVelocity, zAxis);
        adjustment.y = 0f;

        adjustment = Vector3.ClampMagnitude(adjustment, acceleration * Time.deltaTime);

        float turningFactor = rotationCurve.Evaluate(velocity.magnitude / maxSpeed);
        velocity += xAxis * (adjustment.x * turningFactor) + zAxis * (adjustment.z * 1);
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
    }


    private void EvaluateCollision(Collision collision)
    {
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
            }
        }
    }


    private Vector3 ProjectDirectionOnPlane(Vector3 direction, Vector3 normal)
    {
        return (direction - normal * Vector3.Dot(direction, normal)).normalized;
    }

    private float GetMinDot(int layer)
    {
        return (stairsMask & (1 << layer)) == 0 ? minGroundDotProduct : minStairsDotProduct;
    }
}
