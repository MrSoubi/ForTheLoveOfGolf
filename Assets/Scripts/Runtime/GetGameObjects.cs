using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGameObjects : MonoBehaviour
{
    public static GetGameObjects instance;

    public List<Coin> collectibles = new List<Coin>();
    public List<Hole> holes = new List<Hole>();
    public List<Door> doors = new List<Door>();
    public List<PannelCollectible> pannels = new List<PannelCollectible>();
    public List<Challenge> challenges = new List<Challenge>();

    public List<bool> coinsObject = new List<bool>();
    public List<bool> holesObject = new List<bool>();
    public List<bool> doorsObject = new List<bool>();
    public List<bool> pannelsObject = new List<bool>();
    public List<bool> challengesObject = new List<bool>();

    private void Awake()
    {
        instance = this;

        int indexCoins = 0;
        int indexHoles = 0;
        int indexDoors = 0;
        int indexPannels = 0;
        int indexChallenges = 0;

        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out Coin currentCollectible))
            {
                collectibles.Add(currentCollectible);

                if (SaveManager.coinsObject.Count > 0 && SaveManager.coinsObject[indexCoins])
                {
                    coinsObject.Add(true);
                }
                else
                {
                    coinsObject.Add(false);
                }

                indexCoins += 1;
            }
            else if (tmp[i].TryGetComponent(out Hole currentHole))
            {
                holes.Add(currentHole);

                if (SaveManager.holesObject.Count > 0 && SaveManager.holesObject[indexHoles])
                {
                    holesObject.Add(true);
                }
                else
                {
                    holesObject.Add(false);
                }

                indexHoles += 1;
            }
            else if (tmp[i].TryGetComponent(out Door currentDoor))
            {
                doors.Add(currentDoor);

                if (SaveManager.doorsObject.Count > 0 && SaveManager.doorsObject[indexDoors])
                {
                    doorsObject.Add(true);
                }
                else
                {
                    doorsObject.Add(false);
                }

                indexDoors += 1;
            }
            else if (tmp[i].TryGetComponent(out PannelCollectible currentPannel))
            {
                pannels.Add(currentPannel);

                if (SaveManager.pannelsObject.Count > 0 && SaveManager.pannelsObject[indexPannels])
                {
                    pannelsObject.Add(true);
                }
                else
                {
                    pannelsObject.Add(false);
                }
                
                indexPannels += 1;
            }
            else if (tmp[i].TryGetComponent(out Challenge currentChallenge))
            {
                challenges.Add(currentChallenge);

                if (SaveManager.challengesObject.Count > 0 && SaveManager.challengesObject[indexChallenges])
                {
                    challengesObject.Add(true);
                }
                else
                {
                    challengesObject.Add(false);
                }

                indexChallenges += 1;
            }
        }
    }
}
