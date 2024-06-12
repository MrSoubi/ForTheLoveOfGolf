using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [Header("Setting")]
    [SerializeField] private bool startingPoint;

    [Header("Reference")]
    public ParticleSystem particle;
    public ParticleSystem particle2;

    private void Start()
    {
        if (startingPoint) StartCoroutine(Utils.Delay(() => CheckpointManager.instance?.SetCheckpoint(true, transform.position + Vector3.up, this), 0.05f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) SetCheckpoint();
    }

    /// <summary>
    /// D�finit le checkpoint
    /// </summary>
    public void SetCheckpoint()
    {
        CheckpointManager.instance?.SetCheckpoint(false, transform.position + Vector3.up, this);
    }

    /// <summary>
    /// Change le mat�riel du chekpoint
    /// </summary>
    /// <param name="material"></param>
    public void ChangeMaterial(Material material)
    {
        if(material != null) GetComponentInChildren<MeshRenderer>().material  = material;
    }
}
