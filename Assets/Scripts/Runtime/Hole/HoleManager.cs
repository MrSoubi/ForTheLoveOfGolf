using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public CollectibleManager collectibleManager;
    [SerializeField] private Material flagMaterial;
    [SerializeField] private Material flagMaterialComplete;

    Dictionary<int, HoleStatistic> holeInGame = new Dictionary<int, HoleStatistic>();
    public List<Hole> holesCount = new List<Hole>();

    private void Awake()
    {
        Hole[] holes = FindObjectsByType<Hole>(FindObjectsSortMode.None);
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
            holeInGame[id].wasFinish = true;
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

        if (IsEveryHoleFinished())
        {
            print("Every hole was finished");
        }
    }

    bool IsEveryHoleFinished()
    {
        for (int i = 0; i < holeInGame.Count; i++)
        {
            if (!holeInGame[i].wasFinish) return false;
        }
        return true;
    }
}

[System.Serializable]
public class HoleStatistic
{
    public bool wasFinish = false;
}