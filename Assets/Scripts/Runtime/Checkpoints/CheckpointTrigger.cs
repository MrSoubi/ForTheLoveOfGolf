using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool startingPoint;

    private void Start()
    {
        if (startingPoint) StartCoroutine(Utils.Delay(() => CheckpointManager.instance.SetCheckpoint(transform.position + Vector3.up *1.5f ,this),0.05f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player") SetCheckpoint();
    }

    public void SetCheckpoint()
    {
        CheckpointManager.instance.SetCheckpoint(transform.position + Vector3.up * 1.5f, this);
    }

    public void ChangeMaterial(Material material)
    {
        GetComponentInChildren<MeshRenderer>().material  = material;
    }
}
