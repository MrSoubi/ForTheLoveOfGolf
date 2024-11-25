using UnityEngine;

public class Inputs : MonoBehaviour
{
    [Header("Output events")]
    public RSE_EnableCursor EnableCursor;
    public RSE_DisableCursor DisableCursor;

    [Header("Ref Menu")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject optionsMenu;

    private GameObject players;

    private void Start()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        for (int i = 0; i < tmp.Length; i++)
        {
            if(tmp[i].TryGetComponent(out PlayerController player))
            {
                players = player.gameObject;

                break;
            }
        }
    }

    private void Update()
    {
        if(Input.GetButtonDown("Menu"))
        {
            if(optionsMenu != null && optionsMenu.activeInHierarchy)
            {
                optionsMenu.SetActive(false);

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }
            else if(panelMenu != null && !panelMenu.activeInHierarchy)
            {
                Time.timeScale = 0.0f;

                panelMenu.SetActive(true);
                cameraManager.brain.m_IgnoreTimeScale = false;
                EnableCursor.TriggerEvent();
            }
            else if(panelMenu != null && panelMenu.activeInHierarchy)
            {
                Time.timeScale = 1.0f;

                panelMenu.SetActive(false);
                cameraManager.brain.m_IgnoreTimeScale = true;
                DisableCursor.TriggerEvent();

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }  
        }

        if (Input.GetButtonDown("Respawn") && Time.timeScale > 0)
        {
            if (ChallengeManager.instance.currentChallenge != null) ChallengeManager.instance.currentChallenge.Respawn(players);
            else if(GameManager.instance != null) GameManager.instance.Respawn(players);
        }
    }
}
