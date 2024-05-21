using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    public GameObject[] Doors;

    // TEMP
    public int door;

    private void Update()
    {
        if(Doors.Length > 0)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Doors[door].GetComponent<Door>().OpenDoor();
            }
        }
    }
}
