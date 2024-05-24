using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerZone : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera VCam;
    
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered");
        if (other.CompareTag("Player"))
        {
            Debug.Log("player entered");
            VCam.enabled = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            VCam.enabled = false;
        }
    }
}
