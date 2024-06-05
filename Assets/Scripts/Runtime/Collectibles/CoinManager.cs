using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Particle")]
    [SerializeField] private ParticleSystem starsParticle;

    [Header("__DEBUG__")]
    public int coinQuantity;
    public int coinCollected;
    public List<Coin> coinLists = new List<Coin>();

    public static CoinManager instance;

    [HideInInspector]
    public Action<int> onCollectedCoin;
    [HideInInspector]
    public AudioSource sfx;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void AddCoin(Coin coin)
    {
        coinLists.Add(coin);
        coinQuantity += coin.value;
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
        sfx.Play();
        onCollectedCoin?.Invoke(coinCollected);
        Instantiate(starsParticle, coin.mesh.transform.position, coin.mesh.transform.rotation);
        coin.mesh.GetComponent<Animator>().Play("CoinGetAnimation");
        StartCoroutine(Utils.Delay(() => instance.RemoveCoin(coin), 0.2f));
    }
}