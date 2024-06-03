using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance { get; private set; }

    private Vector3 checkPoints;
    private CheckpointTrigger lastTrigger;
    [SerializeField][Tooltip("0)Unvalidate checkpoint, 1)Validate checkpoint ")] private Material[] materials;

    public void Awake() => instance = Utils.Singleton(this, instance, () => Destroy(gameObject));


    public void SetCheckpoint(Vector3 newCheckPoints, CheckpointTrigger trigger)
    {
        if (newCheckPoints != checkPoints) 
        { 
            checkPoints = newCheckPoints;
            if (lastTrigger != null) lastTrigger.ChangeMaterial(materials[0]);
            lastTrigger = trigger;
            lastTrigger.ChangeMaterial(materials[1]);
        }
    }

    public Vector3 GetRespawnPoint() => checkPoints;
}
