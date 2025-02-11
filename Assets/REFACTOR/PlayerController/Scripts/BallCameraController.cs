using UnityEngine;
using UnityEngine.InputSystem;

public class BallCameraController : MonoBehaviour
{
    [Header("Cible")]
    public Transform target; // Référence à la balle

    [Header("Paramètres de la caméra")]
    public float distance = 5f; // Distance entre la caméra et la balle
    public float heightOffset = 1.5f; // Hauteur de la caméra par rapport à la balle
    public float rotationSpeed = 3f; // Sensibilité de la rotation
    public float zoomSpeed = 2f;
    public float minDistance = 2f;
    public float maxDistance = 10f;

    [Header("Limites de l'angle vertical")]
    public float minVerticalAngle = -20f;
    public float maxVerticalAngle = 80f;

    private float yaw = 0f; // Rotation horizontale (gauche/droite)
    private float pitch = 20f; // Rotation verticale (haut/bas)

    private Vector2 lookInput; // Stockage de l'entrée du joueur pour la rotation

    private PlayerInput playerInput;

    void Start()
    {
        if (target == null)
        {
            Debug.LogError("Aucune cible assignée à la caméra !");
            return;
        }

        playerInput = GetComponent<PlayerInput>();

        // Initialisation des angles de rotation
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = Mathf.Clamp(angles.x, minVerticalAngle, maxVerticalAngle);
    }

    void LateUpdate()
    {
        if (target == null) return;

        // Appliquer la rotation en fonction de l'entrée du joueur
        yaw += lookInput.x * rotationSpeed * Time.deltaTime;
        pitch -= lookInput.y * rotationSpeed * Time.deltaTime;
        pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);

        // Calculer la nouvelle position orbitale
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        // Positionner le GameObject parent à la position de la balle
        transform.position = target.position + Vector3.up * heightOffset;

        // Définir la position et la rotation de la caméra
        transform.position += offset;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }

    // Appelé par le système d'Input
    public void OnLook(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            Vector2 delta = context.ReadValue<Vector2>();
            yaw += delta.x * rotationSpeed * Time.deltaTime;
            pitch -= delta.y * rotationSpeed * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minVerticalAngle, maxVerticalAngle);
        }
    }
}
