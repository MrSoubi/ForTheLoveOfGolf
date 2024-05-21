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
     
    // TEMP
    private IEnumerator Temp()
    {
        yield return new WaitForSeconds(2);

        AddCollectible(0);
    }

    private void Start()
    {
        if(collectibles.Length > 0)
        {
            textCollectibleCounter.text = numberCollectible.ToString() + "/" + collectibles.Length;

            // TEMP
            StartCoroutine(Temp());
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
