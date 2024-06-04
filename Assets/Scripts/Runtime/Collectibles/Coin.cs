using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [Header("References")]
    public GameObject mesh;

    [Header("Coin Value")]
    public int value;

    [HideInInspector] public CollectibleManager CollectibleManager;
    [HideInInspector] public int index;

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