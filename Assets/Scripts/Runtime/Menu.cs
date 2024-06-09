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

    [SerializeField] private AudioSource sfx;

    private AsyncOperation ao;

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

    private IEnumerator ChangeLevel()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        ao.allowSceneActivation = true;
    }

    public void StartGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        ao = SceneManager.LoadSceneAsync(level);
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    public void Continue()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        if (SaveManager.LoadFile())
        {
            ao = SceneManager.LoadSceneAsync(level);
            ao.allowSceneActivation = false;

            StartCoroutine(ChangeLevel());
        }
    }

    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Application.Quit();
    }
}
