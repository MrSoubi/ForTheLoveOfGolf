using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private AudioSource sfx;

    private AsyncOperation ao;

    /// <summary>
    /// Change le cursor en main lorsque la souris passe dessus
    /// </summary>
    /// <param name="button"></param>
    public void CursorEnter(Button button)
    {
        if (button.interactable)
        {
            Vector2 hotspot = new Vector2(handCursor.width / 2.5f, handCursor.height / 100f);
            Cursor.SetCursor(handCursor, hotspot, CursorMode.Auto);
        }
    }

    /// <summary>
    /// Change le cursor en default lorsque la souris ne passe plus dessus
    /// </summary>
    /// <param name="button"></param>
    public void CursorExit(Button button)
    {
        if (button.interactable) Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Lorsque qu'un bouton est appuiller
    /// </summary>
    private void ButtonPress()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();
    }

    /// <summary>
    /// Cache le menu et retire la pause
    /// </summary>
    public void Resume()
    {
        ButtonPress();

        CameraManager.Instance.brain.m_IgnoreTimeScale = true;
        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

        Time.timeScale = 1.0f;
        gameObject.SetActive(false);
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    /// <summary>
    /// Change de niveau
    /// </summary>
    private IEnumerator ChangeLevel()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Time.timeScale = 1.0f;
        ao.allowSceneActivation = true;
    }

    /// <summary>
    /// Réinitialise le niveau actuelle
    /// </summary>
    public void Restart()
    {
        ButtonPress();

        CursorManager.instance.SetCursorVisibility(false);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

        ao = SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    /// <summary>
    /// Sauvegarde le jeu
    /// </summary>
    public void SaveGame()
    {
        ButtonPress();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        SaveManager.SaveToFile();
    }

    public void Options()
    {
        ButtonPress();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        optionsMenu.SetActive(true);
    }

    /// <summary>
    /// Retourne au Menu
    /// </summary>
    public void Menu()
    {
        ButtonPress();

        CursorManager.instance.SetCursorVisibility(true);
        CursorManager.instance.SetCursorLockMod(CursorLockMode.None);

        ao = SceneManager.LoadSceneAsync("Menu");
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    /// <summary>
    /// Quite le jeu
    /// </summary>
    public void Quit()
    {
        ButtonPress();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        Application.Quit();
    }
}
