using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    public static CursorManager instance
    {
        get; private set;
    }

    private void Awake() => instance = this.Singleton(instance, () => Destroy(gameObject));

    private void Start()
    {
        SetCursorVisibility(false);
        SetCursorLockMod(CursorLockMode.Locked);
    }

    public void SetCursorVisibility(bool isVisible)
    {
        Cursor.visible = isVisible;
    }

    public void SetCursorLockMod(CursorLockMode lockMode)
    {
        Cursor.lockState = lockMode;
    }
}