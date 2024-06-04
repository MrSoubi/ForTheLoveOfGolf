using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public CollectibleManager collectibleManager;
    [SerializeField] private Material flagMaterial;
    [SerializeField] private Material flagMaterialComplete;

    Dictionary<int, HoleStatistic> holeInGame = new Dictionary<int, HoleStatistic>();
    [HideInInspector]
    public List<Hole> holesCount = new List<Hole>();

    private void Awake()
    {
        Hole[] holes = FindObjectsByType<Hole>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].holeManager = this;
            holes[i].SetID(i);
            holeInGame.Add(i, new HoleStatistic());
            holesCount.Add(holes[i]);
            collectibleManager.holeValue += 1;
        }
    }

    public void FinishSelectedHole(int id)
    {
        if (!holeInGame[id].wasFinish)
        {
            collectibleManager.holeCount += 1;
            //SaveManager.holes = collectibleManager.holeCount;
            //SaveManager.holesObject[id] = true;
            holeInGame[id].wasFinish = true;

            collectibleManager.onCollectedHole?.Invoke(collectibleManager.holeCount);

            if(holesCount[id].finish)
            {
                if(collectibleManager.collectibleCount >= collectibleManager.collectibleValue && collectibleManager.holeCount >= collectibleManager.holeValue)
                {
                    holesCount[id].flag.GetComponent<MeshRenderer>().material = flagMaterialComplete;
                }
                else
                {
                    holesCount[id].flag.GetComponent<MeshRenderer>().material = flagMaterial;
                }
            }
            else
            {
                holesCount[id].flag.GetComponent<MeshRenderer>().material = flagMaterial;
            }

            collectibleManager.RefreshInterface();
        }
    }
}

[System.Serializable]
public class HoleStatistic
{
    public bool wasFinish = false;
}