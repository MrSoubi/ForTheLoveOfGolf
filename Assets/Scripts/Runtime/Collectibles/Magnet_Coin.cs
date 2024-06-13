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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;

            StartCoroutine(MoveCoin());
        }
    }

    private IEnumerator MoveCoin()
    {
        if (coin != null && player != null)
        {
            coin.transform.position = Vector3.Lerp(coin.transform.position, player.transform.position, forceFactor * Time.deltaTime);
        }

        yield return new WaitForSeconds(0.01f);

        forceFactor *= IncrementationMultiplicator;
        StartCoroutine(MoveCoin());
    }
}
