using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject CollectibleManager;

    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player") // Perf -> utiliser CompareTag() (Manu)
        {
            CollectibleManager.GetComponent<CollectibleManager>().AddCollectible(index);
        }
    }
}
