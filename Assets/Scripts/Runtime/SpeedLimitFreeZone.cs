using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimitFreeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PC_MovingSphere>().UnClampVelocity();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PC_MovingSphere>().ClampVelocity();
        }
    }
}
