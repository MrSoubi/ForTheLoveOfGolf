using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] CheckpointTrigger checkpointExit;
    [SerializeField] Vector3 respawnPointOffset;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointExit)
        {
            StartCoroutine(TeleportCoroutine(other.gameObject));
        }
    }

    IEnumerator TeleportCoroutine(GameObject player)
    {
        if (player.TryGetComponent(out PC_MovingSphere controller))
        {
            controller.Block();

            yield return new WaitForSeconds(.4f);

            UIManager.instance.FadeIn();

            yield return new WaitForSeconds(1.2f);

            checkpointExit.SetCheckpoint();
            controller.Teleport(checkpointExit.transform.position + respawnPointOffset);

            UIManager.instance.FadeOut();

            yield return new WaitForSeconds(.8f);
            controller.UnBlock(true);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(checkpointExit) Gizmos.DrawSphere(checkpointExit.transform.position + respawnPointOffset, .1f);
    }
}