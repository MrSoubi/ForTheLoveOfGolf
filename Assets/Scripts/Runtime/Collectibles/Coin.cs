using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Coin : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem stars;
    public GameObject mesh;

    [Header("Settings")]
    public int value;

    [Header("__DEBUG__")]
    private bool isPickingUp;

    private void Start()
    {
        StartCoroutine(Utils.Delay(() => CoinManager.instance.AddCoin(this), .001f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !isPickingUp)
        {
            isPickingUp = true;
            CoinManager.instance.CollectCoin(this);
        }
    }
}