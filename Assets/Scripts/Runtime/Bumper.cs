using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour
{
    [SerializeField]
    private float intensity;

    public void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.transform.CompareTag("Player"))
        {
            //other.transform.parent.GetComponent<PlayerController>().Bump(intensity, transform.forward);
        }
    }
}
