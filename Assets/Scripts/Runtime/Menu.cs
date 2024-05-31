using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public Texture2D handCursor;

    public void CursorEnter()
    {
        Cursor.SetCursor(handCursor, new Vector2(200, 0), CursorMode.Auto);
    }

    public void CursorExit()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public void StartGame()
    {
        EventSystem.current.SetSelectedGameObject(null);

        SceneManager.LoadScene("LevelArea");
    }

    public void Continue()
    {
        EventSystem.current.SetSelectedGameObject(null);
    }

    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Application.Quit();
    }
}
