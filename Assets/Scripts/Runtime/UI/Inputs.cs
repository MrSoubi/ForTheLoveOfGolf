using UnityEngine;

public class Inputs : MonoBehaviour
{
    [Header("Ref Menu")]
    [SerializeField] private GameObject panelMenu;
    [SerializeField] private GameObject optionsMenu;

    private GameObject players;

    private void Start()
    {
        GameObject[] tmp = FindObjectsByType<GameObject>(FindObjectsSortMode.None);

        for (int i = 0; i < tmp.Length; i++)
        {
            if(tmp[i].TryGetComponent(out PC_MovingSphere player))
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
                CameraManager.Instance.brain.m_IgnoreTimeScale = false;
                CursorManager.instance.SetCursorVisibility(true);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.None);
            }
            else if(panelMenu != null && panelMenu.activeInHierarchy)
            {
                Time.timeScale = 1.0f;

                panelMenu.SetActive(false);
                CameraManager.Instance.brain.m_IgnoreTimeScale = true;
                CursorManager.instance.SetCursorVisibility(false);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

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
