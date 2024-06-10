using UnityEngine;

public class Inputs : MonoBehaviour
{
    [Header("Ref Menu")]
    [SerializeField] private GameObject panelMenu;

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
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(!panelMenu.activeInHierarchy)
            {
                Time.timeScale = 0.0f;

                panelMenu.SetActive(true);
                CameraManager.Instance.brain.m_IgnoreTimeScale = false;
                CursorManager.instance.SetCursorVisibility(true);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.None);
            }
            else
            {
                Time.timeScale = 1.0f;

                panelMenu.SetActive(false);
                CameraManager.Instance.brain.m_IgnoreTimeScale = true;
                CursorManager.instance.SetCursorVisibility(false);
                CursorManager.instance.SetCursorLockMod(CursorLockMode.Locked);

                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            }  
        }

        if (Input.GetKeyDown(KeyCode.R) && Time.timeScale > 0)
        {
            GameManager.instance.Respawn(players);
        }
    }
}
