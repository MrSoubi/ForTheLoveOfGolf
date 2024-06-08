using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if(ChallengeManager.instance.currentChallenge != null)
            {
                ChallengeManager.instance.currentChallenge.Respawn(other.gameObject);
            }
            else
            {
                GameManager.instance?.Respawn(other.gameObject);
            }
        }
    }
}