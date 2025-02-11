using UnityEngine;

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

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Si aucune caméra n'est assignée, on cherche la caméra principale
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

        // Gérer le saut
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(groundNormal * jumpForce, ForceMode.Impulse);
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
        if (cameraTransform == null) return; // Vérifier qu'on a bien une caméra

        float moveInput = 0f;
        float strafeInput = 0f; // Pour le mouvement latéral

        if (Input.GetKey(KeyCode.Z)) moveInput = 1f;
        if (Input.GetKey(KeyCode.S)) moveInput = -1f;
        if (Input.GetKey(KeyCode.Q)) strafeInput = -1f; // Gauche
        if (Input.GetKey(KeyCode.D)) strafeInput = 1f; // Droite

        // Projeter les directions de la caméra sur le sol
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, groundNormal).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, groundNormal).normalized;

        // Appliquer le déplacement vers l'avant/arrière
        Vector3 moveForce = forward * moveInput * moveSpeed;

        // Appliquer un déplacement latéral (strafe)
        Vector3 strafeForce = right * strafeInput * moveSpeed;

        // Appliquer les forces au Rigidbody
        rb.AddForce(moveForce + strafeForce, ForceMode.Acceleration);
    }

}
