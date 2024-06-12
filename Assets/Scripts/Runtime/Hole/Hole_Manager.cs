using System;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material completedMaterial;
    [SerializeField] private Material goldenMaterial;

    [Header("Ref Audio")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource goldenSfx;

    [Header("__DEBUG__")]
    public int holeQuantity;
    public int holeCollected;
    public List<Hole> holesList = new List<Hole>();

    public Action onCollectedHole;

    public static HoleManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    /// <summary>
    /// Ajoute un troue au compteur et dans la liste
    /// </summary>
    /// <param name="hole"></param>
    public void AddHole(Hole hole)
    {
        holesList.Add(hole);
        holeQuantity++;
    }

    /// <summary>
    /// Finis un troue
    /// </summary>
    /// <param name="hole"></param>
    public void CompleteHole(Hole hole)
    {
        holeCollected++;
        onCollectedHole?.Invoke();

        if ((hole.isGoldenFlag) && (holeCollected >= holeQuantity))
        {
            if(goldenSfx != null) goldenSfx.Play();
            if(goldenMaterial != null) hole.GetFlagMesh1().material = goldenMaterial;
            if(goldenMaterial != null) hole.GetFlagMesh2().material = goldenMaterial;
        }
        else
        {
            if (sfx != null)  sfx.Play();
            if(completedMaterial != null) hole.GetFlagMesh1().material = completedMaterial;
            if (completedMaterial != null) hole.GetFlagMesh2().material = completedMaterial;
        }
    }
}