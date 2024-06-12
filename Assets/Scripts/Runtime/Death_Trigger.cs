using UnityEngine;

public class DeathTrigger : MonoBehaviour
{
    [Header("Ref Audio")]
    [SerializeField] private AudioSource sfx;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(sfx != null) sfx.Play();

            if(ChallengeManager.instance.currentChallenge != null) ChallengeManager.instance.currentChallenge.Respawn(other.gameObject);
            else GameManager.instance?.Respawn(other.gameObject);
        }
    }
}