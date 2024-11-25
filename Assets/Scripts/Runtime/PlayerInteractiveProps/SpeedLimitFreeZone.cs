using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedLimitFreeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().UnClampVelocity();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().ClampVelocity();
        }
    }
}
