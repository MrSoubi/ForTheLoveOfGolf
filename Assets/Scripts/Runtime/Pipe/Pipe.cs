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
                StartCoroutine(TeleportCoroutine(other.gameObject));
            }
            else print("Il n'y a pas de sortie au tuyau !");
        }
    }

    IEnumerator TeleportCoroutine(GameObject player)
    {        
        player.TryGetComponent(out PlayerController controller);
        if (controller)
        {
            controller.Block();

            yield return new WaitForSeconds(.4f);

            UIManager.instance?.FadeIn();

            yield return new WaitForSeconds(1.2f);

           controller.Teleport(backroomCheckpoint.transform.position);
            backroomCheckpoint.SetCheckpoint();

            UIManager.instance?.FadeOut();

            yield return new WaitForSeconds(.8f);
            controller.UnBlock();
        }
    }

    private void OnDrawGizmos()
    {
        if(backroomCheckpoint) Gizmos.DrawSphere(backroomCheckpoint.transform.position, .2f);
    }
}
