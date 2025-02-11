using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BallController : MonoBehaviour
{
    [Header("Mouvement")]
    public float moveSpeed = 10f;
    public float turnSpeed = 5f;

    [Header("Saut")]
    public float jumpForce = 5f;
    public LayerMask groundLayer;

    [Header("Caméra")]
    public Transform cameraTransform; // Référence à la caméra principale

    private Rigidbody rb;
    private Vector3 groundNormal = Vector3.up; // Normale du sol
    private bool isGrounded;

    private Vector2 moveInput; // Stockage de l'entrée du joystick gauche
    private bool jumpPressed = false; // Détecte si le saut a été pressé

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        UpdateGroundNormal();

        // Vérifier si la balle touche le sol
        isGrounded = Physics.Raycast(transform.position, -groundNormal, 0.6f, groundLayer);

        // Gérer le saut avec le Gamepad
        if (jumpPressed && isGrounded)
        {
            rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
            jumpPressed = false; // Réinitialisation après le saut
        }
    }

    void FixedUpdate()
    {
        MoveBall();
    }

    void UpdateGroundNormal()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 1f, groundLayer))
        {
            groundNormal = hit.normal; // Récupérer la normale du sol
        }
    }

    void MoveBall()
    {
        if (cameraTransform == null) return;

        // Projeter les directions de la caméra sur le sol
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, groundNormal).normalized;

        // Calcul des forces basées sur le joystick gauche
        Vector3 moveForce = forward * moveInput.y * moveSpeed;
        Vector3 strafeForce = right * moveInput.x * moveSpeed;

        // Appliquer la force combinée
        rb.AddForce(moveForce + strafeForce, ForceMode.Acceleration);
    }

    // Gestion du joystick gauche
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    // Gestion du saut (touche "A" du Gamepad)
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            jumpPressed = true;
        }
    }
}
