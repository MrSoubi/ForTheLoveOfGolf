using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("__DEBUG__")]
    public int coinQuantity;
    public int coinCollected;
    public List<Coin> coinLists = new List<Coin>();

    public Action onCollectedCoin;

    public static CoinManager instance;

    [SerializeField] private AudioSource sfx;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void AddCoin(Coin coin)
    {      
        coinLists.Add(coin);
        coinQuantity++;
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
        onCollectedCoin?.Invoke();

        if(sfx != null)
        {
            sfx.Play();
        }

        if (coin.stars != null)
        {
            Instantiate(coin.stars, coin.transform.position, coin.transform.rotation);
        }

        RemoveCoin(coin);
    }
}