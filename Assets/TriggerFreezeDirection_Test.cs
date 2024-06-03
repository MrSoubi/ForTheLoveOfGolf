using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerFreezeDirection_Test : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.GetComponent<PC_MovingSphere>().FreezeDirection();
    }

    private void OnTriggerExit(Collider other)
    {
        other.GetComponent<PC_MovingSphere>().UnFreezeDirection();
    }
}
