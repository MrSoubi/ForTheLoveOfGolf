using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem stars;

    [Header("__DEBUG__")]
    public int coinQuanity;
    public int coinCollected;
    public List<Coin> coinLists = new List<Coin>();

    public static CoinManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void AddCoin(Coin coin)
    {      
        coinLists.Add(coin);
        coinQuanity++;
    }
    public void RemoveCoin(Coin coin)
    {
        if(coinLists.Count > 0)
        {
            coinLists.Remove(coin);
            Destroy(coin.gameObject);
        }
    }

    public void CollectCoin(Coin coin)
    {
        coinCollected++;
        Instantiate(stars, coin.transform.position, coin.transform.rotation);
        RemoveCoin(coin);
    }
}