using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private Button buttonContinue;

    [Header("Level Direction")]
    public string level;

    private AsyncOperation ao;

    private void Start()
    {
        /*string filePath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Save", "Save.json");

        if (File.Exists(filePath))
        {
            buttonContinue.GetComponent<Button>().interactable = true;
            buttonContinue.GetComponent<EventTrigger>().enabled = true;
        }
        else
        {
            buttonContinue.GetComponent<Button>().interactable = false;
            buttonContinue.GetComponent<EventTrigger>().enabled = false;
        }*/
    }

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

    /// <summary>
    /// Charge la sauvegarde du jeu
    /// </summary>
    public void Continue()
    {
        ButtonPress();

        if (SaveManager.LoadFile())
        {
            ao = SceneManager.LoadSceneAsync(level);
            ao.allowSceneActivation = false;

            StartCoroutine(ChangeLevel());
        }
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
