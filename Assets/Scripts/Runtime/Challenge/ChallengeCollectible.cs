using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeCollectible : MonoBehaviour
{
    [HideInInspector] public Challenge challengeReference;
    public int value;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            challengeReference.AddCoin(value);
            gameObject.SetActive(false);
        }
    }
}