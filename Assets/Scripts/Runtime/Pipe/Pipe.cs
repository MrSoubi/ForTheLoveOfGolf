using System.Collections;
using UnityEngine;

public class Pipe : MonoBehaviour
{
    [SerializeField] GameObject pipeExit;
    [SerializeField] Vector3 respawnPointOffset;

    [SerializeField] private Animator ballContentAnim;
    [SerializeField] private Transform virtualBall;

    private GameObject collision;
    private bool take;

    [SerializeField] private AudioSource sfx;

    private void Start()
    {
        if(virtualBall != null) virtualBall.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && pipeExit != null && !take)
        {
            take = true;
            collision = other.gameObject;
            collision.SetActive(false);
            virtualBall.gameObject.SetActive(true);

            if (sfx != null) sfx.Play();

            ballContentAnim.SetTrigger("EnterAnim");
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

        tmp.Teleport(pipeExit.transform.position + respawnPointOffset);

        UIManager.instance.FadeOut();

        yield return new WaitForSeconds(.8f);
        tmp.UnBlock(true);
        collision = null;
        take = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if(pipeExit) Gizmos.DrawSphere(pipeExit.transform.position + respawnPointOffset, .1f);
    }
}