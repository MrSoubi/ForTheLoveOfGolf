using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : MonoBehaviour
{
    [SerializeField] Vector3 respawnPoint;

    [SerializeField] int ID;
    [HideInInspector] public HoleManager holeManager;

    public void SetID(int id) => ID = id;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.transform.CompareTag("Player"))
        {
            holeManager.FinishSelectedHole(ID);

            print("REPLACE PLAYER ON RESPAWNPOINT");
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(transform.position + respawnPoint, .1f);
    }
}