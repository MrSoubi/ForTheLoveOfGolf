using UnityEngine;

public class BallVisualController : MonoBehaviour
{
    public Transform physicsBall; // Référence au GameObject physique
    public float rotationSpeedMultiplier = 1f; // Facteur pour ajuster la vitesse de rotation

    private Rigidbody rb; // Référence au Rigidbody de la balle physique
    private Vector3 previousPosition;

    void Start()
    {
        if (physicsBall == null)
        {
            Debug.LogError("Référence à la balle physique manquante !");
            return;
        }

        rb = physicsBall.GetComponent<Rigidbody>();
        previousPosition = physicsBall.position;
    }

    void Update()
    {
        if (rb == null) return;

        // Suivre la position de la balle physique
        transform.position = physicsBall.position;

        // Calcul de la rotation
        Vector3 movement = physicsBall.position - previousPosition;
        if (movement.magnitude > 0.001f) // Éviter les divisions par zéro
        {
            // Trouver l'axe de rotation (perpendiculaire au mouvement et à l'axe Y)
            Vector3 rotationAxis = Vector3.Cross(movement.normalized, Vector3.up);

            // Calculer l'angle en fonction de la vitesse et INVERSER la rotation
            float rotationAngle = -(movement.magnitude / transform.localScale.x) * Mathf.Rad2Deg * rotationSpeedMultiplier;

            // Appliquer la rotation
            transform.Rotate(rotationAxis, rotationAngle, Space.World);
        }

        previousPosition = physicsBall.position; // Stocker la position actuelle pour le prochain calcul
    }
}

