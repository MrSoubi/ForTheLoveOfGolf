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
    public TextMeshProUGUI textHoleCounter;

    [Header("Collectible")]
    public int collectibleCount;
    public int collectibleValue;

    [Header("Hole")]
    public int holeCount;
    public int holeValue;

    bool showCollectibleCount = true;
    [SerializeField] Animator collectibleCountAnim;

    public Action<int> onCollectedCoin;
    public Action<int> onCollectedHole;

    public static CollectibleManager instance { get; private set; }

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));

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
            collectibleValue += collectibles[i].value;
            collectibles[i].CollectibleManager = this;
            collectibles[i].index = i;
        }

        RefreshInterface();
        ShowCollectibleCount();
    }

    /// <summary>
    /// Met � jour l'affichage du compteur
    /// </summary>
    public void RefreshInterface()
    {
        if (textCollectibleCounter != null && collectibles.Count > 0)
        {
            textCollectibleCounter.text = collectibleCount.ToString() + "/" + collectibleValue + " Coins";
            textHoleCounter.text = holeCount.ToString() + "/" + holeValue + " Holes";
            textCollectibleCounter.text = collectibleCount.ToString() + "/" + collectibleValue;
            textHoleCounter.text = holeCount.ToString() + "/" + holeValue;
        }
    }

    /// <summary>
    /// Met � jour les index dans la liste
    /// </summary>
    private void ResetCollectibleIndex()
    {
        for (int i = 0; i < collectibles.Count; i++)
        {
            collectibles[i].index = i;
        }
    }

    /// <summary>
    /// Detruit le GameObject puis appelle la fonction ResetCollectibleIndex qui met � jour les index dans la liste
    /// </summary>
    /// <param name="index">L'index de la piece</param>
    public void DelCollectible(int index)
    {
        Destroy(collectibles[index].gameObject);
        collectibles.RemoveAt(index);

        ResetCollectibleIndex();
    }

    /// <summary>
    /// Ajoute la valeur de la pi�ce dans le compteur et l'affiche puis appelle la fonction DelCollectible qui detruit le GameObject
    /// </summary>
    /// <param name="index">L'index de la pi�ce</param>
    /// <param name="value">La valeur de la pi�ce</param>
    public void AddCollectible(int index, int value)
    {
        collectibleCount += value;
        onCollectedCoin?.Invoke(collectibleCount);

        RefreshInterface();

        DelCollectible(index);
    }

    public void ShowCollectibleCount()
    {
        showCollectibleCount = true;
        collectibleCountAnim.SetBool("Show", showCollectibleCount);
    }
    public void HidCollectibleCount()
    {
        showCollectibleCount = false;
        collectibleCountAnim.SetBool("Show", showCollectibleCount);
    }
}
