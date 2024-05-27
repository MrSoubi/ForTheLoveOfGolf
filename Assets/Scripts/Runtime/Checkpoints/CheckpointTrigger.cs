using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool startingPoint;

    private void Start()
    {
        if (startingPoint) Utils.Delay(() => CheckpointManager.instance.SetCheckpoint(transform.position),0.05f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") SetCheckpoint();
    }

    public void SetCheckpoint()
    {
        CheckpointManager.instance.SetCheckpoint(transform.position);
    }
}
