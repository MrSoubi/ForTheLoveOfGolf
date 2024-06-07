using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material completedMaterial;
    [SerializeField] private Material goldenMaterial;

    [Header("__DEBUG__")]
    public int holeQuantity;
    public int holeCollected;

    public List<Hole> holesList = new List<Hole>();

    public Action onCollectedHole;

    public static HoleManager instance;

    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource goldenSfx;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void AddHole(Hole hole)
    {
        holesList.Add(hole);
        holeQuantity++;
    }

    public void RemoveHole(Hole hole)
    {
        if(holesList.Count > 0) holesList.Remove(hole);
    }

    public void CompleteHole(Hole hole)
    {
        holeCollected++;
        onCollectedHole?.Invoke();

        if ((hole.isGoldenFlag) && (holeCollected >= holeQuantity) && (CoinManager.instance.coinCollected >= CoinManager.instance.coinQuantity))
        {
            if(goldenSfx != null) goldenSfx.Play();
            hole.GetFlagMesh().material = goldenMaterial;
        }
        else
        {
            if (sfx != null)  sfx.Play();
            hole.GetFlagMesh().material = completedMaterial;
        }
    }
}