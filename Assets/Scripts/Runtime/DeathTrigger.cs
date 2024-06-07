using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    //public Challenge challenge;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
        }
    }
}