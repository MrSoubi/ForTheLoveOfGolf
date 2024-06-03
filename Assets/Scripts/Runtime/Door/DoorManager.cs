using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [HideInInspector]
    public List<Door> doors = new List<Door>();

    private void Start()
    {
        doors = GetGameObjects.instance.doors;
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].index = i;

            if (SaveManager.doors[i])
            {
                doors[i].OpenDoor();
            }
        }
    }
}
