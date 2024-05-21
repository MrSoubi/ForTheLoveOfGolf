using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    public GameObject CollectibleManager;

    public int index;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            CollectibleManager.GetComponent<CollectibleManager>().AddCollectible(index);
        }
    }
}
