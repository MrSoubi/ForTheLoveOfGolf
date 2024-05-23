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

    public int collectibleCount;

    /// <summary>
    /// Met à jour l'affichage du compteur
    /// </summary>
    private void Start()
    {
        if(collectibles.Length > 0)
        {
            textCollectibleCounter.text = collectibleCount.ToString() + "/" + collectibles.Length;
        }
    }

    /// <summary>
    /// Ajoute une pièce dans le compteur puis appelle la fonction DelCollectible qui detruit le GameObject
    /// </summary>
    /// <param name="index">L'index de la piece</param>
    public void AddCollectible(int index)
    {
        collectibleCount += 1;

        textCollectibleCounter.text = collectibleCount.ToString() + "/" + collectibles.Length;

        DelCollectible(index);
    }

    /// <summary>
    /// Detruit le GameObject
    /// </summary>
    /// <param name="index">L'index de la piece</param>
    public void DelCollectible(int index)
    {
        Destroy(collectibles[index]);
    }
}
