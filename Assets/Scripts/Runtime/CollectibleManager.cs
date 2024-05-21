using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CollectibleManager : MonoBehaviour
{
    public TextMeshProUGUI textCollectibleCounter;

    public GameObject[] collectibles;

    public int numberCollectible;

    private void Start()
    {
        if(collectibles.Length > 0)
        {
            textCollectibleCounter.text = numberCollectible.ToString() + "/" + collectibles.Length;
        }
    }

    public void AddCollectible(int index)
    {
        numberCollectible += 1;

        textCollectibleCounter.text = numberCollectible.ToString() + "/" + collectibles.Length;

        DelCollectible(index);
    }

    public void DelCollectible(int index)
    {
        Destroy(collectibles[index]);
    }
}
