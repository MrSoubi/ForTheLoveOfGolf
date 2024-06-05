using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material completedMaterial;
    [Header("__DEBUG__")]
    public int holeQuantity;
    public int holeCompleted;

    public List<Hole> holesList = new List<Hole>();

    public static HoleManager instance;

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
        holeCompleted += 1;
        hole.GetFlagMesh().material = completedMaterial;
    }
}