using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public TimeChallenge Challenges;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            
            GameManager.instance.Respawn(other.gameObject);
        }
    }
}