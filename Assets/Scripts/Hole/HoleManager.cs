using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    Dictionary<int, HoleStatistic> holeInGame = new Dictionary<int, HoleStatistic>();

    private void Start()
    {
        Hole[] holes = FindObjectsByType<Hole>(FindObjectsSortMode.None);
        for (int i = 0; i < holes.Length; i++)
        {
            holes[i].holeManager = this;
            holes[i].SetID(i);
            holeInGame.Add(i, new HoleStatistic());
        }
    }

    public void FinishSelectedHole(int id)
    {
        holeInGame[id].wasFinish = true;
    }
}

[System.Serializable]
public class HoleStatistic
{
    public bool wasFinish = false;
}