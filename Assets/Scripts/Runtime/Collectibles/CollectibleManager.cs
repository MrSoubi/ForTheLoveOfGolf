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

    public int numberCollectible; // Nomenclature : collectibleCount ?

    private void Start()
    {
        if(collectibles.Length > 0)
        {
            textCollectibleCounter.text = numberCollectible.ToString() + "/" + collectibles.Length;
        }
    }

    // Un peu de documentation sur ces deux fontions pour savoir ce qu'elles ajoutent et supprime, c'est pas super clair pour moi (Manu)
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
