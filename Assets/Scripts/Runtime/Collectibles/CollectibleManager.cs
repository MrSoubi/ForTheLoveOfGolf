using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CollectibleManager : MonoBehaviour
{
    [HideInInspector]
    public List<Collectible> collectibles = new List<Collectible>();

    [Header("Interface")]
    public TextMeshProUGUI textCollectibleCounter;

    [Header("Collectible")]
    public int collectibleCount;
    public int collectibleValue;

    [Header("Hole")]
    public int holeCount;
    public int holeValue;

    private void Awake()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out Collectible currentCollectible))
            {
                collectibles.Add(currentCollectible);
            }
            if (tmp[i].TryGetComponent(out Buttons currentButton))
            {
                currentButton.collectibleManager = this;
            }
        }
    }

    private void Start()
    {
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].CollectibleManager = this;
            collectibles[i].index = i;
        }

        RefreshInterface();
    }

    /// <summary>
    /// Met à jour l'affichage du compteur
    /// </summary>
    private void RefreshInterface()
    {
        if (textCollectibleCounter != null && collectibles.Count > 0)
        {
            collectibleValue = 0;

            for (int i = 0; i < collectibles.Count; i++)
            {
                collectibleValue += collectibles[i].value;
            }

            textCollectibleCounter.text = collectibleCount.ToString() + "/" + collectibleValue;
        }
    }

    /// <summary>
    /// Met à jour les index dans la liste
    /// </summary>
    private void ResetCollectibleIndex()
    {
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].index = i;
        }
    }

    /// <summary>
    /// Detruit le GameObject puis appelle la fonction ResetCollectibleIndex qui met à jour les index dans la liste
    /// </summary>
    /// <param name="index">L'index de la piece</param>
    public void DelCollectible(int index)
    {
        Destroy(collectibles[index].gameObject);
        collectibles.RemoveAt(index);

        ResetCollectibleIndex();
    }

    /// <summary>
    /// Ajoute la valeur de la pièce dans le compteur et l'affiche puis appelle la fonction DelCollectible qui detruit le GameObject
    /// </summary>
    /// <param name="index">L'index de la pièce</param>
    /// <param name="value">La valeur de la pièce</param>
    public void AddCollectible(int index, int value)
    {
        collectibleCount += value;

        RefreshInterface();

        DelCollectible(index);
    }
}
