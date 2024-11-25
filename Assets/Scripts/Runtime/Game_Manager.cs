using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    /// <summary>
    /// Téléporte le joueur à son checkpoint
    /// </summary>
    /// <param name="currentPlayer"></param>
    public void Respawn(GameObject currentPlayer)
    {
        PlayerController tmp = currentPlayer.GetComponent<PlayerController>();

        tmp.Block();
        tmp.Teleport(CheckpointManager.instance.GetRespawnPoint());
        tmp.UnBlock(true);
    }
}