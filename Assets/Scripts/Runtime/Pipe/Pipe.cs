using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] CheckpointTrigger checkpointExit;
    [SerializeField] Vector3 respawnPointOffset;

    [SerializeField] private Animator ballContentAnim;
    [SerializeField] private Transform virtualBall;

    private GameObject collision;
    private bool take;

    private void Start()
    {
        virtualBall.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && checkpointExit && !take)
        {
            take = true;
            collision = other.gameObject;
            collision.SetActive(false);
            virtualBall.gameObject.SetActive(true);

            ballContentAnim.SetTrigger("WinAnim");
            StartCoroutine(TeleportCoroutine());

            virtualBall.transform.localScale = collision.transform.localScale / transform.localScale.x;
            virtualBall.GetComponent<MeshFilter>().mesh = collision.GetComponentInChildren<MeshFilter>().mesh;
            virtualBall.GetComponent<MeshRenderer>().materials = collision.GetComponentInChildren<MeshRenderer>().materials;
        }
    }

    IEnumerator TeleportCoroutine()
    {
        yield return new WaitForSeconds(.5f);

        virtualBall.gameObject.SetActive(false);
        PC_MovingSphere tmp = collision.GetComponent<PC_MovingSphere>();

        tmp.Block();

        yield return new WaitForSeconds(.4f);

        UIManager.instance.FadeIn();

        yield return new WaitForSeconds(1.2f);

        collision.SetActive(true);

        tmp.Teleport(checkpointExit.transform.position + respawnPointOffset);

        UIManager.instance.FadeOut();

        yield return new WaitForSeconds(.8f);
        tmp.UnBlock(true);
        collision = null;
        take = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(checkpointExit) Gizmos.DrawSphere(checkpointExit.transform.position + respawnPointOffset, .1f);
    }
}