using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool startingPoint;

    [Header("Reference")]
    [SerializeField] private ParticleSystem particle;

    private void Start()
    {
        if (startingPoint) StartCoroutine(Utils.Delay(() => CheckpointManager.instance?.SetCheckpoint(transform.position + Vector3.up *1.5f ,this),0.05f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) SetCheckpoint();
    }

    /// <summary>
    /// Définit le checkpoint
    /// </summary>
    public void SetCheckpoint()
    {
        CheckpointManager.instance?.SetCheckpoint(transform.position + Vector3.up, this);

        if (CheckpointManager.instance.sfx != null) CheckpointManager.instance.sfx.Play();
        if (particle != null) particle.Play();
    }

    /// <summary>
    /// Change le matériel du chekpoint
    /// </summary>
    /// <param name="material"></param>
    public void ChangeMaterial(Material material)
    {
        if(material != null) GetComponentInChildren<MeshRenderer>().material  = material;
    }
}
