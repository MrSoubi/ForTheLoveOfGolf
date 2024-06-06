using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeChallengeCoin : MonoBehaviour
{
    [Header("References")]
    public GameObject mesh;

    [Header("Settings")]
    public int value;

    [Header("__DEBUG__")]
    private bool isPickingUp;
    public Action onCollected;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isPickingUp)
        {
            onCollected?.Invoke();
            isPickingUp = true;
        }
    }
}