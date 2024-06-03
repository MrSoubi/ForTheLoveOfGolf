using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
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

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1.0f;

        gameObject.SetActive(false);
        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);
    }

    public void Restart()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1.0f;

        SceneManager.LoadScene("LevelArea");
        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);
    }

    public void SaveGame()
    {
        EventSystem.current.SetSelectedGameObject(null);

        SaveManager.SaveToFile();
    }

    public void Menu()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Time.timeScale = 1.0f;

        SceneManager.LoadScene("Menu");
        CursorManager.instance.SetCursorVisibility(true);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.None);
    }

    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Application.Quit();
    }
}
