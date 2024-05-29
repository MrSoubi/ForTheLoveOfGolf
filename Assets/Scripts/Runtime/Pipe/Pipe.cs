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
<<<<<<< Updated upstream
                print("CHANGER LA FONCTION DE TP");
                other.transform.position = backroomCheckpoint.transform.position;
                backroomCheckpoint.SetCheckpoint();
=======
                StartCoroutine(TeleportCoroutine(other.gameObject));
>>>>>>> Stashed changes
            }
            else print("Il n'y a pas de sortie au tuyau !");
        }
    }

    IEnumerator TeleportCoroutine(GameObject player)
    {
        // Freez player pos
        // Fade in

        yield return new WaitForSeconds(1.2f);

        player.GetComponent<PlayerController>().Teleport(backroomCheckpoint.transform.position);
        backroomCheckpoint.SetCheckpoint();

        // Fade out
        // unfreez player pos
    }

    private void OnDrawGizmos()
    {
        if(backroomCheckpoint) Gizmos.DrawSphere(backroomCheckpoint.transform.position, .2f);
    }
}
