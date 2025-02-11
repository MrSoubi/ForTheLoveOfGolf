using UnityEngine;
using UnityEngine.InputSystem;

public class BallShotController : MonoBehaviour
{
    [Header("Références")]
    public Rigidbody rb;
    public Transform cameraTransform; // Caméra pour déterminer la direction du tir
    public LineRenderer trajectoryRenderer; // Pour afficher la courbe

    [Header("Paramètres du tir")]
    public float maxChargeTime = 2f; // Temps max de charge du tir
    public float maxShotForce = 20f; // Force max appliquée à la balle
    public int trajectoryPoints = 20; // Nombre de points sur la courbe
    public float launchAngle = 20f; // **Angle de tir vers le haut (en degrés)**

    private bool isBallFrozen = false; // Indique si la balle est figée
    private bool isChargingShot = false; // Indique si le tir est en train de charger
    private float chargeTime = 0f; // Temps de charge du tir

    private Vector3 savedVelocity;
    private Vector3 savedAngularVelocity;

    void Start()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody>();
        }
        trajectoryRenderer.enabled = false; // Désactiver la trajectoire au début
    }

    void Update()
    {
        if (isChargingShot)
        {
            // Augmente la charge du tir tant que B est enfoncé
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Clamp(chargeTime, 0, maxChargeTime);

            // Met à jour la courbe de trajectoire
            UpdateTrajectory();
        }
    }

    // Appelé lorsque la gâchette gauche est enfoncée (fige la balle)
    public void OnHoldBall(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            isBallFrozen = true;

            savedVelocity = rb.linearVelocity;
            savedAngularVelocity = rb.angularVelocity;

            rb.isKinematic = true; // On fige la balle
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            if (!isChargingShot) // Si B n'est pas pressé, reprendre le mouvement
            {
                isBallFrozen = false;

                rb.isKinematic = false;
                rb.linearVelocity = savedVelocity;
                rb.angularVelocity = savedAngularVelocity;
            }
        }
    }

    // Appelé lorsque B est pressé (début du tir)
    public void OnChargeShot(InputAction.CallbackContext context)
    {
        if (isBallFrozen && context.phase == InputActionPhase.Started)
        {
            isChargingShot = true;
            chargeTime = 0f;
            trajectoryRenderer.enabled = true;
        }
        else if (isBallFrozen && context.phase == InputActionPhase.Canceled)
        {
            // Relâchement du tir
            ShootBall();
            isChargingShot = false;
            trajectoryRenderer.enabled = false;
        }
    }

    void ShootBall()
    {
        // Calcul de la puissance du tir
        float shotPower = (chargeTime / maxChargeTime) * maxShotForce;

        // Direction du tir = direction de la caméra avec un **angle vers le haut**
        Vector3 shotDirection = Quaternion.AngleAxis(-launchAngle, cameraTransform.right) * cameraTransform.forward;

        // Appliquer la force
        rb.isKinematic = false;
        rb.AddForce(shotDirection * shotPower, ForceMode.Impulse);

        // Réinitialisation
        isBallFrozen = false;
        chargeTime = 0f;
    }

    void UpdateTrajectory()
    {
        Vector3 startPosition = transform.position;
        Vector3 initialVelocity = GetLaunchVelocity();

        trajectoryRenderer.positionCount = trajectoryPoints;

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float time = (i / (float)trajectoryPoints) * (2f * initialVelocity.y / -Physics.gravity.y); // Approximation du temps de vol
            Vector3 position = startPosition + initialVelocity * time + 0.5f * Physics.gravity * time * time;
            trajectoryRenderer.SetPosition(i, position);
        }
    }


    Vector3 GetLaunchVelocity()
    {
        float shotPower = (chargeTime / maxChargeTime) * maxShotForce;

        // Incliner le tir vers le haut avec un angle
        Vector3 shotDirection = Quaternion.AngleAxis(-launchAngle, cameraTransform.right) * cameraTransform.forward;

        // Calculer la vélocité initiale en appliquant la force
        return shotDirection * shotPower;
    }
}