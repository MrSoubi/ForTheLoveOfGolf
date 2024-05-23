using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance { get; private set; }

    private Vector3 checkPoints;
    private CheckpointTrigger lastTrigger;

    public void Awake() => instance = Utils.Singleton(this, instance, () => Destroy(gameObject));

    public void SetCheckpoint(Vector3 newCheckPoints, CheckpointTrigger currentTrigger)
    {
        if (newCheckPoints != checkPoints) 
        {
            currentTrigger.SetTexture(true);
            if (lastTrigger) lastTrigger.SetTexture(false);
            lastTrigger = currentTrigger;
            checkPoints = newCheckPoints;
        }
    }

    public Vector3 GetRespawnPoint() => checkPoints;
}
