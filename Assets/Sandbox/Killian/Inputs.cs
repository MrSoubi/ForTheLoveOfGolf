using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Inputs : MonoBehaviour
{
    public GameObject player;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(Time.timeScale > 0f)
            {
                Time.timeScale = 0.0f;
            }
            else
            {
                Time.timeScale = 1.0f;
            }  
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            GameManager.instance.Respawn(player);
        }
    }
}
