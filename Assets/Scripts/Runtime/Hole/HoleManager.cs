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

    private void Start()
    {
        holesCount = GetGameObjects.instance.holes;

        for (int i = 0; i < holesCount.Count; i++)
        {
            holesCount[i].holeManager = this;
            holesCount[i].SetID(i);
            holeInGame.Add(i, new HoleStatistic());
            collectibleManager.holeValue += 1;
        }
    }

    public void FinishSelectedHole(int id)
    {
        if (!holeInGame[id].wasFinish)
        {
            collectibleManager.holeCount += 1;
            SaveManager.holes = collectibleManager.holeCount;
            GetGameObjects.instance.holesObject[id] = true;
            holeInGame[id].wasFinish = true;

            collectibleManager.onCollectedHole?.Invoke(collectibleManager.holeCount);

            if(holesCount[id].finish)
            {
                if(collectibleManager.collectibleCount >= collectibleManager.collectibleValue && collectibleManager.holeCount >= collectibleManager.holeValue)
                {
                    holesCount[id].gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = flagMaterialComplete;
                }
                else
                {
                    holesCount[id].gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = flagMaterial;
                }
            }
            else
            {
                holesCount[id].gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = flagMaterial;
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