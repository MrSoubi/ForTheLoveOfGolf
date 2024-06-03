using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }
    public void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void Respawn(GameObject currentPlayer)
    {
        PC_MovingSphere tmp = currentPlayer.GetComponent<PC_MovingSphere>();

        tmp.Block();
        tmp.Teleport(CheckpointManager.instance.GetRespawnPoint());
        tmp.UnBlock(true);
    }
}