using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectible : MonoBehaviour
{
    [HideInInspector] 
    public CollectibleManager CollectibleManager;

    public int index;

    [Header("Value Collectible")]
    public int value;

    [Header("Animation Speed")]
    public float speed;

    private void Update()
    {
        transform.Rotate(0, speed * Time.deltaTime, 0);
    }

    /// <summary>
    /// Quand le joueur est d�tecter, il appelle le manager et ajoute la valeur de la pi�ce
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