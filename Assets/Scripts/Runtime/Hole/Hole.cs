using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] Vector3 respawnPoint;

    public GameObject flag;

    [SerializeField] int ID;
    [HideInInspector] public HoleManager holeManager;
    public bool finish;

    GameObject collision;

    public void SetID(int id) => ID = id;

    [SerializeField] Animator ballContentAnim;
    [SerializeField] Transform virtualBall;
    [SerializeField] Transform flagVisual;

    private void Start()
    {
        virtualBall.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            holeManager.FinishSelectedHole(ID);
            collision = other.gameObject;
            collision.SetActive(false);
            virtualBall.gameObject.SetActive(true);

            ballContentAnim.SetTrigger("WinAnim");
            StartCoroutine(SpawnPoint());

            virtualBall.transform.localScale = collision.transform.localScale;
            virtualBall.GetComponent<MeshFilter>().mesh = collision.GetComponentInChildren<MeshFilter>().mesh; 
            virtualBall.GetComponent<MeshRenderer>().materials = collision.GetComponentInChildren<MeshRenderer>().materials;
        }
    }

    IEnumerator SpawnPoint()
    {
        yield return new WaitForSeconds(1.1f);

        collision.SetActive(true);
        virtualBall.gameObject.SetActive(false);


        PC_MovingSphere tmp = collision.GetComponent<PC_MovingSphere>();

        tmp.Block();
        tmp.Teleport(transform.position + respawnPoint);
        tmp.UnBlock(true);

        collision = null;

        flagVisual.Bump();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + respawnPoint, .1f);
    }
}