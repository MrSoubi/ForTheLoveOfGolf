using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(ChallengeManager.instance.currentChallenge != null) ChallengeManager.instance.currentChallenge.Respawn(other.gameObject);
            else GameManager.instance?.Respawn(other.gameObject);
        }
    }
}