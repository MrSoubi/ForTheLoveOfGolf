using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] Pipe backroomPipe;
    public Vector3 respawnPoint;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (backroomPipe)
            {
                StartCoroutine(TeleportCoroutine(other.gameObject));
            }
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

            controller.Teleport(backroomPipe.transform.position + backroomPipe.respawnPoint);

            UIManager.instance?.FadeOut();

            yield return new WaitForSeconds(.8f);
            controller.UnBlock();
        }
    }

    private void OnDrawGizmos()
    {
        if(backroomPipe) Gizmos.DrawSphere(backroomPipe.respawnPoint, .2f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + respawnPoint, .1f);
    }
}
