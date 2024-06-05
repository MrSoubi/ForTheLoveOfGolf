using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material completedMaterial;
    [SerializeField] private Material completedFullMaterial;
    [Header("__DEBUG__")]
    public int holeQuantity;
    public int holeCompleted;

    public List<Hole> holesList = new List<Hole>();

    public static HoleManager instance;

    [HideInInspector]
    public Action<int> onCollectedHole;

    [HideInInspector]
    public AudioSource sfx;
    [HideInInspector]
    public AudioSource sfxFinish;

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
        if (!hole.completed)
        {
            holeCompleted += 1;
            onCollectedHole?.Invoke(holeCompleted);
        }

        if (hole.finishFlag)
        {
            if(holeQuantity >= holeCompleted && CoinManager.instance.coinCollected >= CoinManager.instance.coinQuantity)
            {
                sfxFinish.Play();
                hole.GetFlagMesh().material = completedFullMaterial;
            }
            else
            {
                sfx.Play();
                hole.GetFlagMesh().material = completedMaterial;
            }
        }
        else
        {
            sfx.Play();
            hole.GetFlagMesh().material = completedMaterial;
        }
        
    }
}