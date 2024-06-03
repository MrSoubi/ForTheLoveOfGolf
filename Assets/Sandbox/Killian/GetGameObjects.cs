using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameObjects : MonoBehaviour
{
    public static GetGameObjects instance;

    public List<Collectible> collectibles = new List<Collectible>();
    public List<Hole> holes = new List<Hole>();
    public List<Door> doors = new List<Door>();
    public List<PannelCollectible> pannels = new List<PannelCollectible>();
    public List<Challenge> challenges = new List<Challenge>();

    private void Start()
    {
        instance = this;
    }

    private void Awake()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out Collectible currentCollectible))
            {
                collectibles.Add(currentCollectible);
                SaveManager.coinsObject.Add(false);
            }
            if (tmp[i].TryGetComponent(out Hole currentHole))
            {
                holes.Add(currentHole);
                SaveManager.holesObject.Add(false);
            }
            if (tmp[i].TryGetComponent(out Door currentDoor))
            {
                doors.Add(currentDoor);
                SaveManager.doors.Add(false);
            }
            if (tmp[i].TryGetComponent(out PannelCollectible currentPannel))
            {
                pannels.Add(currentPannel);
                SaveManager.pannel.Add(false);
            }
            if (tmp[i].TryGetComponent(out Challenge currentChallenge))
            {
                challenges.Add(currentChallenge);
                SaveManager.challenges.Add(false);
            }
        }
    }
}
