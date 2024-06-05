using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    public Challenge Challenges;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(Challenges != null && Challenges.active)
            {
                Challenges.EndChallenge();
            }
            
            GameManager.instance.Respawn(other.gameObject);
        }
    }
}