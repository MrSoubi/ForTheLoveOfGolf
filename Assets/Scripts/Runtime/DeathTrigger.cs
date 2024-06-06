using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public Challenge challenge;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(challenge != null && challenge.active)
            {
                challenge.EndChallenge();
            }
            
            GameManager.instance?.Respawn(other.gameObject);
        }
    }
}