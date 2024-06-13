using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] GameObject optionsMenu;
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private Button buttonContinue;

    [Header("Level Direction")]
    public string level;

    private AsyncOperation ao;

    /// <summary>
    /// Change le cursor en main lorsque la souris passe dessus
    /// </summary>
    /// <param name="button"></param>
    public void CursorEnter(Button button)
    {
        if(button.interactable)
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
    /// Change de niveau
    /// </summary>
    private IEnumerator ChangeLevel()
    {
        yield return new WaitForSecondsRealtime(0.2f);

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        ao.allowSceneActivation = true;
    }

    /// <summary>
    /// Commence le jeu
    /// </summary>
    public void StartGame()
    {
        ButtonPress();

        ao = SceneManager.LoadSceneAsync(level);
        ao.allowSceneActivation = false;

        StartCoroutine(ChangeLevel());
    }

    public void Options()
    {
        ButtonPress();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        optionsMenu.SetActive(true);
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
