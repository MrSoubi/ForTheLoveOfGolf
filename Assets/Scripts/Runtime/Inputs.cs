using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Inputs : MonoBehaviour
{
    public GameObject players;
    public GameObject panelMenu;

    private void Start()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);
        for (int i = 0; i < tmp.Length; i++)
        {
            if(tmp[i].TryGetComponent(out PC_MovingSphere player))
            {
                players = player.gameObject;
                break;
            }
        }
    }

    private void Update()
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
