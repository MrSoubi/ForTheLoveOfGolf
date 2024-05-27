using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] CheckpointTrigger backroomCheckpoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (backroomCheckpoint)
            {
                print("CHANGER LA FONCTION DE TP");
                other.transform.position = backroomCheckpoint.transform.position;
                backroomCheckpoint.SetCheckpoint();
            }
            else print("Il n'y a pas de sortie au tuyau !");
        }
    }

    private void OnDrawGizmos()
    {
        if(backroomCheckpoint) Gizmos.DrawSphere(backroomCheckpoint.transform.position, .2f);
    }
}
