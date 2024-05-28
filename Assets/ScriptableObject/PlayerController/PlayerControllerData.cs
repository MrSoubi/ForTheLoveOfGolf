using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerData", menuName = "SCO/PlayerControllerData")]
public class PlayerControllerData : ScriptableObject
{
    public enum EnvironmentEffect
    {
        NORMAL,
        ONWOOD,
        MOIST,
        CHILLED,
        SLIMY
    }

    public float moveSpeed;
    public float maxSpeed;
    public float airMultiplier;
    public float rotationSpeed;
    public float gravityForce;

    public float shootForce;
    public int shootCharges;
    public int maxShootCharges;

    public KeyCode aimingInput = KeyCode.Mouse1;
    public KeyCode shootInput = KeyCode.Mouse0;

    public Material materialOpaque;
    public Material materialTransparent;

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

    public float shootingAngle;

    public AnimationCurve yCurve;

    public Vector3 contactPoint;
    public bool complexDetection;
    public float groundDetectionLength = 0.025f;

    private Vector3 savedVelocity;
    private bool isFreezed;

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
}
