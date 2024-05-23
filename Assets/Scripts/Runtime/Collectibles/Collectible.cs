using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [HideInInspector] 
    public CollectibleManager CollectibleManager;

    [HideInInspector]
    public int index;

    public int value;

    /// <summary>
    /// Quand le joueur est détecter, il appelle le manager et ajoute la valeur de la pièce
    /// </summary>
    /// <param name="other">le collider qui entre en contact</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CollectibleManager.AddCollectible(index, value);
        }
    }
}