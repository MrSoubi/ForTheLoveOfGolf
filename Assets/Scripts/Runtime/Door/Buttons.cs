using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buttons : MonoBehaviour
{

    [Header("Door")]
    [SerializeField] private Door door;
    [SerializeField] private Vector3 positionBtnAnchor;

    [Header("Animation")]
    [SerializeField] private int animeDuration;

    /// <summary>
    /// Quand le joueur est détecter, il ouvre la porte uniquement si l'objectif est bon
    /// </summary>
    /// <param name="other">le collider qui entre en contact</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            door?.TriggerOpen();
            transform.DOMove(new Vector3(positionBtnAnchor.x, positionBtnAnchor.y - 0.3f, positionBtnAnchor.z), animeDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOMove(new Vector3(positionBtnAnchor.x, positionBtnAnchor.y, positionBtnAnchor.z), animeDuration);
        }
    }
}
