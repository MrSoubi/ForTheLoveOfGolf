using System.Collections;
using Cinemachine;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public RSE_DeclareHole declareHole;
    public RSE_CompleteHole completeHole;

    [Header("References")]
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private Animator ballContentAnim;
    [SerializeField] private Transform virtualBall;
    [SerializeField] private MeshRenderer flagMesh;
    [SerializeField] private MeshRenderer flagMesh2;
    [SerializeField] private ParticleSystem completedParticle;

    [Header("Settings")]
    [SerializeField] private float animeDuration;
    [SerializeField] private Vector3 respawnPoint;

    public bool isGoldenFlag;

    [Header("Cinematique Settings")]
    [SerializeField] private float delayBeforeActivation;
    [SerializeField] private float delayAfterActivation;

    [Header("DEBUG")]
    public bool completed;

    private GameObject collision;

    private void Start()
    {
        declareHole.TriggerEvent?.Invoke();

        virtualBall.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !completed)
        {
            completed = true;

            PC_MovingSphere pc = other.GetComponent<PC_MovingSphere>();

            pc.ToggleRoll(true);
            pc.SetDirection(Vector3.zero);
            pc.Block();

            CameraManager.Instance.ActivateCamera(cam);

            StartCoroutine(Utils.Delay(() =>
            {
                completeHole.TriggerEvent.Invoke();

                collision = other.gameObject;
                collision.SetActive(false);
                virtualBall.gameObject.SetActive(true);

                ballContentAnim.SetTrigger("WinAnimation");
                StartCoroutine(SpawnPoint());

                virtualBall.transform.localScale = collision.transform.localScale / transform.localScale.x;
                virtualBall.GetComponent<MeshFilter>().mesh = collision.GetComponentInChildren<MeshFilter>().mesh;
                virtualBall.GetComponent<MeshRenderer>().materials = collision.GetComponentInChildren<MeshRenderer>().materials;

            }, delayBeforeActivation));

            StartCoroutine(Utils.Delay(() =>
            {
                CameraManager.Instance.DeActivateCurrentCamera();
                pc.UnBlock(true);
            }, animeDuration + delayAfterActivation));
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + respawnPoint, 0.1f);
    }

    /// <summary>
    /// Retour le drapeau 1
    /// </summary>
    public MeshRenderer GetFlagMesh1() 
    {
        if (flagMesh != null) return flagMesh;
        else return null;
    }

    /// <summary>
    /// Retour le drapeau 2
    /// </summary>
    public MeshRenderer GetFlagMesh2()
    {
        if (flagMesh2 != null) return flagMesh2;
        else return null;
    }

    /// <summary>
    /// T�l�porte le joueur � un endroit pr�cis
    /// </summary>
    private void Teleport()
    {
        PC_MovingSphere tmp = collision.GetComponent<PC_MovingSphere>();

        tmp.Block();
        tmp.Teleport(transform.position + respawnPoint);
        tmp.UnBlock(true);
    }

    /// <summary>
    /// Joue l'animation et t�l�porte le joueur
    /// </summary>
    private IEnumerator SpawnPoint()
    {
        yield return new WaitForSeconds(1.15f);

        if (completedParticle != null)
        {
            ParticleSystem particle = Instantiate(completedParticle, transform);
            particle.transform.localScale = transform.localScale;
        }

        collision.SetActive(true);
        virtualBall.gameObject.SetActive(false);

        Teleport();

        collision = null;

        if(flagMesh != null) flagMesh.GetComponent<Transform>().Bump();
        if (flagMesh2 != null) flagMesh2.GetComponent<Transform>().Bump();
    }
}