using UnityEngine;
using static UnityEngine.ParticleSystem;

public class CheckpointManager : MonoBehaviour
{
    [Header("Ref materials")]
    [SerializeField][Tooltip("0)Unvalidate checkpoint, 1)Validate checkpoint ")] private Material[] materials;

    [Header("Ref Audio")]
    public AudioSource sfx;

    private Vector3 checkPoints;
    private CheckpointTrigger lastTrigger;

    public static CheckpointManager instance { get; private set; }

    public void Awake() => instance = Utils.Singleton(this, instance, () => Destroy(gameObject));

    /// <summary>
    /// Met � jour le checkpoint
    /// </summary>
    /// <param name="newCheckPoints"></param>
    /// <param name="trigger"></param>
    public void SetCheckpoint(bool startingPoint, Vector3 newCheckPoints, CheckpointTrigger trigger)
    {
        if (newCheckPoints != checkPoints) 
        {
            if(!startingPoint)
            {
                if (sfx != null) sfx.Play();

                if (trigger.particle != null)
                {
                    trigger.particle.transform.localScale = trigger.transform.localScale;
                    trigger.particle2.transform.localScale = trigger.transform.localScale;
                    trigger.particle.Play();
                }
            }

            checkPoints = newCheckPoints;
            if (lastTrigger != null) lastTrigger.ChangeMaterial(materials[0]);
            lastTrigger = trigger;
            lastTrigger.ChangeMaterial(materials[1]);
        }
    }

    /// <summary>
    /// Renvoie le checkpoint actuelle
    /// </summary>
    public Vector3 GetRespawnPoint() => checkPoints;
}
