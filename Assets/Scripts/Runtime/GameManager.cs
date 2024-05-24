using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public static GameManager instance { get; private set; }
    public void Awake() => instance = this.Singleton(instance, () => Destroy(gameObject));

    public void Respawn(GameObject currentPlayer)
    {
        //Remplace par fonction tp joueur
        //print(currentPlayer);
        //currentPlayer.transform.position = CheckpointManager.instance.GetRespawnPoint();
    }

}
