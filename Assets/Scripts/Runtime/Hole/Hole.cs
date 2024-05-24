using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    public Vector3 respawnPoint;

    [SerializeField] int ID;
    [HideInInspector] public HoleManager holeManager;

    public void SetID(int id) => ID = id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            holeManager.FinishSelectedHole(ID);

            SpawnPoint(other);
        }
    }

    public void NewFlag()
    {

    }

    private void SpawnPoint(Collider other)
    {
        other.transform.position = transform.position + respawnPoint;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + respawnPoint, .1f);
    }
}