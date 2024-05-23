using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public CollectibleManager CollectibleManager;

    Dictionary<int, HoleStatistic> holeInGame = new Dictionary<int, HoleStatistic>();

    private void Start()
    {
        Hole[] holes = FindObjectsByType<Hole>(FindObjectsSortMode.None);
        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].holeManager = this;
            holes[i].SetID(i);
            holeInGame.Add(i, new HoleStatistic());
            CollectibleManager.holeCount += 1;
        }
    }

    public void FinishSelectedHole(int id)
    {
        if (!holeInGame[id].wasFinish)
        {
            CollectibleManager.holeValue += 1;
            holeInGame[id].wasFinish = true;
        }
    }
}

[System.Serializable]
public class HoleStatistic
{
    public bool wasFinish = false;
}