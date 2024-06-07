using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buttons : MonoBehaviour
{

    [Header("Door")]
    [SerializeField] private Door door;

    [Header("Animation")]
    [SerializeField] private int animeDuration;
    [SerializeField] private Vector3 posButton;

    [Header("Audio")]
    [SerializeField] private AudioSource sfx;

    private void Start()
    {
        posButton = transform.position;
    }

    /// <summary>
    /// Quand le joueur est détecter, il ouvre la porte uniquement si l'objectif est bon
    /// </summary>
    /// <param name="other">le collider qui entre en contact</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sfx.Play();
            door?.TriggerOpen();
            transform.DOMove(new Vector3(posButton.x, posButton.y - 0.2f, posButton.z), animeDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            sfx.Play();
            transform.DOMove(new Vector3(posButton.x, posButton.y, posButton.z), animeDuration);
        }
    }
}
