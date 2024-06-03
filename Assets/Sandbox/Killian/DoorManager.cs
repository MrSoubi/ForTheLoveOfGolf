using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    [HideInInspector]
    public List<Door> doors = new List<Door>();

    private void Awake()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < tmp.Length; i++)
        {
            if (tmp[i].TryGetComponent(out Door currentDoor))
            {
                doors.Add(currentDoor);
            }
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
