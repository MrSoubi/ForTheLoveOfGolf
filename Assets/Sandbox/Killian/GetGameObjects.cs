using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameObjects : MonoBehaviour
{
    private void Start()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out Collectible currentCollectible))
            {
                SaveManager.coinsObject.Add(false);
            }
            if (tmp[i].TryGetComponent(out Hole currentHole))
            {
                SaveManager.holesObject.Add(false);
            }
            if (tmp[i].TryGetComponent(out Door currentDoor))
            {
                SaveManager.doors.Add(false);
            }
            if (tmp[i].TryGetComponent(out PannelCollectible currentPanneau))
            {
                SaveManager.panneaux.Add(false);
            }
            if (tmp[i].TryGetComponent(out Challenge currentChallenge))
            {
                SaveManager.challenges.Add(false);
            }
        }
    }
}
