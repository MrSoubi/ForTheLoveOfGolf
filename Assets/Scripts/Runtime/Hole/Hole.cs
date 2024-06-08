using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEditor.PackageManager;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Vector3 respawnPoint;
    public bool isGoldenFlag;

    [Header("References")]
    [SerializeField] private Animator ballContentAnim;
    [SerializeField] private Transform virtualBall;
    [SerializeField] private MeshRenderer flagMesh;
    [SerializeField] private MeshRenderer flagMesh2;
    [SerializeField] private ParticleSystem completedParticle;

    [Header("DEBUG")]
    public bool completed;

    GameObject collision;

    public MeshRenderer GetFlagMesh1() 
    {
        if (flagMesh != null) return flagMesh;
        else Debug.LogError("Flag Mesh is null"); return null;
    }

    public MeshRenderer GetFlagMesh2()
    {
        if (flagMesh2 != null) return flagMesh2;
        else Debug.LogError("Flag Mesh is null"); return null;
    }

    private void Start()
    {
        virtualBall.gameObject.SetActive(false);

        StartCoroutine(Utils.Delay(() => HoleManager.instance.AddHole(this), .001f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !completed)
        {
            completed = true;

            if (HoleManager.instance != null) HoleManager.instance.CompleteHole(this);
            else Debug.LogError("No Hole Manager on scene");

            collision = other.gameObject;
            collision.SetActive(false);
            virtualBall.gameObject.SetActive(true);

            ballContentAnim.SetTrigger("WinAnim");
            StartCoroutine(SpawnPoint());

            virtualBall.transform.localScale = collision.transform.localScale / transform.localScale.x;
            virtualBall.GetComponent<MeshFilter>().mesh = collision.GetComponentInChildren<MeshFilter>().mesh; 
            virtualBall.GetComponent<MeshRenderer>().materials = collision.GetComponentInChildren<MeshRenderer>().materials;
        }
    }

    IEnumerator SpawnPoint()
    {
        yield return new WaitForSeconds(1.1f);

        if(completedParticle != null)
        {
            Instantiate(completedParticle, transform).Play();
        }
        
        yield return new WaitForSeconds(0.2f);

        collision.SetActive(true);
        virtualBall.gameObject.SetActive(false);

        PC_MovingSphere tmp = collision.GetComponent<PC_MovingSphere>();

        tmp.Block();
        tmp.Teleport(transform.position + respawnPoint);
        tmp.UnBlock(true);

        collision = null;

        flagMesh.GetComponent<Transform>().Bump();
        flagMesh2.GetComponent<Transform>().Bump();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + respawnPoint, 0.1f);
    }
}