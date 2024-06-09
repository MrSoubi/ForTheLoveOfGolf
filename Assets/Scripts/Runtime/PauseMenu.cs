using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Texture2D handCursor;

    [SerializeField] private AudioSource sfx;

    private AsyncOperation ao;

    public void CursorEnter(Button button)
    {
        if (button.interactable)
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

    public void Resume()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        CameraManager.Instance.brain.m_IgnoreTimeScale = true;
        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    private IEnumerator ChangeLevel()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Time.timeScale = 1.0f;
        ao.allowSceneActivation = true;
    }

    public void Restart()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

        ao = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    public void SaveGame()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        SaveManager.SaveToFile();
    }

    public void Menu()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        CursorManager.instance.SetCursorVisibility(true);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.None);

        ao = SceneManager.LoadSceneAsync("Menu");
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    public void Quit()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Application.Quit();
    }
}
