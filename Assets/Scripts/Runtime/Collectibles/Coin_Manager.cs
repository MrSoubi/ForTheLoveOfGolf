using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [Header("Ref Audio")]
    [SerializeField] private AudioSource sfx;

    [Header("__DEBUG__")]
    public int coinQuantity;
    public int coinCollected;
    public List<Coin> coinLists = new List<Coin>();

    public Action onCollectedCoin;

    public static CoinManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    /// <summary>
    /// Ajoute une pièce au compteur et dans la liste
    /// </summary>
    /// <param name="coin"></param>
    public void AddCoin(Coin coin)
    {      
        coinLists.Add(coin);
        coinQuantity += coin.value;
    }

    /// <summary>
    /// Retire la pièce de la liste et le détruit
    /// </summary>
    /// <param name="coin"></param>
    public void RemoveCoin(Coin coin)
    {
        if(coinLists.Count > 0)
        {
            coinLists.Remove(coin);
            Destroy(coin.gameObject);
        }
    }

    /// <summary>
    /// Ajoute une pièce au compteur
    /// </summary>
    /// <param name="coin"></param>
    public void CollectCoin(Coin coin)
    {
        coinCollected += coin.value;
        onCollectedCoin?.Invoke();

        if(sfx != null) sfx.Play();

        if (coin.stars != null) Instantiate(coin.stars, coin.transform.position, coin.transform.rotation);

        RemoveCoin(coin);
    }
}