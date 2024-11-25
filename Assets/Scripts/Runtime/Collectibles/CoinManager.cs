using System;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    public RSO_CollectedCoins collectedCoins;
    public RSO_TotalCoins totalCoins;

    public RSE_CollectCoin collectCoin;
    public RSE_DeclareCoin declareCoin;

    private void OnEnable()
    {
        declareCoin.TriggerEvent += DeclareCoin;
        collectCoin.TriggerEvent += CollectCoin;

        totalCoins.Value = 0;
        collectedCoins.Value = 0;
    }

    private void OnDisable()
    {
        declareCoin.TriggerEvent -= DeclareCoin;
        collectCoin.TriggerEvent -= CollectCoin;
    }

    private void DeclareCoin(int amount)
    {
        totalCoins.Value += amount;
    }

    private void CollectCoin(int amount)
    {
        collectedCoins.Value += amount;
    }
}