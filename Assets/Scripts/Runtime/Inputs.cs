using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inputs : MonoBehaviour
{
    public GameObject players;
    public GameObject panelMenu;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!panelMenu.activeInHierarchy)
            {
                Time.timeScale = 0.0f;

                panelMenu.SetActive(true);
                CursorManager.instance.SetCursorVisibility(true);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.None);
            }
            else
            {
                Time.timeScale = 1.0f;

                panelMenu.SetActive(false);
                CursorManager.instance.SetCursorVisibility(false);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }  
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.Respawn(players);
        }
    }
}
