using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class Menu : MonoBehaviour
{
    public Button buttonContinue;
    public Texture2D handCursor;

    public string level;

    private void Start()
    {
        string filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Save", "Save.json");
        if (File.Exists(filePath))
        {
            buttonContinue.GetComponent<Button>().interactable = true;
            buttonContinue.GetComponent<EventTrigger>().enabled = true;
        }
        else
        {
            buttonContinue.GetComponent<Button>().interactable = false;
            buttonContinue.GetComponent<EventTrigger>().enabled = false;
        }

    }

    public void CursorEnter(Button button)
    {
        if(button.interactable)
        {
            Cursor.SetCursor(handCursor, new Vector2(200, 0), CursorMode.Auto);
        }
    }

    public void CursorExit(Button button)
    {
        if (button.interactable)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public void StartGame()
    {
        EventSystem.current.SetSelectedGameObject(null);

        SceneManager.LoadScene(level);
    }

    public void Continue()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (SaveManager.LoadFile())
        {
            SceneManager.LoadScene(level);
        }
    }

    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);

        Application.Quit();
    }
}
