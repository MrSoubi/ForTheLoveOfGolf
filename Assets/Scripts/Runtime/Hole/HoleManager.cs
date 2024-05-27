using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleManager : MonoBehaviour
{
    public CollectibleManager CollectibleManager;
    [SerializeField] private Material flagMaterial;

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
            CollectibleManager.holeValue += 1;
        }
    }

    public void FinishSelectedHole(int id)
    {
        if (!holeInGame[id].wasFinish)
        {
            CollectibleManager.holeCount += 1;
            holeInGame[id].wasFinish = true;
            holesCount[id].gameObject.transform.GetChild(0).GetComponent<MeshRenderer>().material = flagMaterial;
        }
    }
}

[System.Serializable]
public class HoleStatistic
{
    public bool wasFinish = false;
}