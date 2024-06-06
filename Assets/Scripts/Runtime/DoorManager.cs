using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [HideInInspector] public List<Door> doors = new List<Door>();

    private void Awake()
    {
        Door[] tmp = FindObjectsByType<Door>(FindObjectsSortMode.None);
        foreach (Door door in tmp)
        {
            doors.Add(door);
        }
    }

    private void Start()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].index = i;
        }
    }
}
