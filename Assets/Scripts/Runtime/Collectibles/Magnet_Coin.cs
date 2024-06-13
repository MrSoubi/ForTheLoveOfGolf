using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Magnet_Coin : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject coin;

    [Header("Settings")]
    [SerializeField] private float forceFactor;
    [SerializeField] private float IncrementationMultiplicator;

    private GameObject player;
    private bool inside;

    private float fact1;
    private float fact2;

    private void Start()
    {
        fact1 = forceFactor;
        fact2 = IncrementationMultiplicator;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inside = true;
            player = other.gameObject;   
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inside = false;
            player = null;
            forceFactor = fact1;
            IncrementationMultiplicator = fact2;
        }
    }

    private IEnumerator test()
    {
        yield return new WaitForSeconds(0.5f);

        forceFactor *= IncrementationMultiplicator;

        if (coin != null && player != null)
        {
            coin.transform.position = Vector3.Lerp(coin.transform.position, player.transform.position, forceFactor * Time.deltaTime);
        }
    }

    private void FixedUpdate()
    {
        if(inside && player != null)
        {
            StartCoroutine(test());
        }
    }
}
