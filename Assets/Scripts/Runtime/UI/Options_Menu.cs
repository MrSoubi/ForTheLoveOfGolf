using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Options_Menu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Texture2D handCursor;
    [SerializeField] private TextMeshProUGUI textMusic;
    [SerializeField] private TextMeshProUGUI textSfx;
    [SerializeField] private AudioMixer mixer;
    [SerializeField] private AudioSource sfx;
    [SerializeField] Slider audioMusic;
    [SerializeField] Slider audioSFX;

    private void Start()
    {
        float valueMusic;
        float valueSfx;

        mixer.GetFloat("MusicVolume", out valueMusic);
        mixer.GetFloat("SFXVolume", out valueSfx);

        valueMusic = (valueMusic + 80) * (100.0f / 80.0f);
        valueSfx = (valueSfx + 80) * (100.0f / 80.0f);

        audioMusic.value = valueMusic;
        audioSFX.value = valueSfx;
    }

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

    public void Music(float val)
    {
        EventSystem.current.SetSelectedGameObject(null);

        textMusic.text = val.ToString() + "%";

        mixer.SetFloat("MusicVolume", val * (80.0f / 100.0f) - 80);
    }

    public void SFX(float val)
    {
        EventSystem.current.SetSelectedGameObject(null);

        textSfx.text = val.ToString() + "%";

        mixer.SetFloat("SFXVolume", val * (80.0f / 100.0f) - 80);
    }

    /// <summary>
    /// Lorsque qu'un bouton est appuiller
    /// </summary>
    private void ButtonPress()
    {
        EventSystem.current.SetSelectedGameObject(null);
        sfx.Play();
    }

    public void Validation()
    {
        ButtonPress();

        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);

        gameObject.SetActive(false);
    }

}
