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

    private void Start()
    {
        StartCoroutine(Utils.Delay(() => CoinManager.instance.AddCoin(this), .001f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            CoinManager.instance.CollectCoin(this);
        }
    }
}