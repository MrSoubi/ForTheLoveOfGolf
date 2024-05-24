using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointTrigger : MonoBehaviour
{
    [SerializeField] private bool startingPoint;

    [SerializeField] private Material[] materials;

    private MeshRenderer meshRenderer;

    private void Awake() => meshRenderer = GetComponentInChildren<MeshRenderer>();

    private void Start()
    {
        if (startingPoint) 
        {
            Utils.Delay(() => CheckpointManager.instance.SetCheckpoint(transform.position, this), 0.01f);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Player") CheckpointManager.instance.SetCheckpoint(transform.position,this);
    }

    public void SetTexture(bool enable)
    {
        meshRenderer.material = materials[enable?1:0];
    }
}
