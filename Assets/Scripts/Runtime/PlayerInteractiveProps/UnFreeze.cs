using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnFreeze : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) other.GetComponent<PC_MovingSphere>().UnFreeze();
    }
}
