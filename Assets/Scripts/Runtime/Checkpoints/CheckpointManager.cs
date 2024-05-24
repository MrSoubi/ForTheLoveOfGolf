using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager instance { get; private set; }

    private Vector3 checkPoints;

    public void Awake() => instance = Utils.Singleton(this, instance, () => Destroy(gameObject));

    public void SetCheckpoint(Vector3 newCheckPoints)
    {
        if (newCheckPoints != checkPoints) checkPoints = newCheckPoints;
    }

    public Vector3 GetRespawnPoint() => checkPoints;
}
