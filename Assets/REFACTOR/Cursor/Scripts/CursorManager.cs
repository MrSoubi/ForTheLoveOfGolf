using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : MonoBehaviour
{
    [SerializeField] RSE_EnableCursor EnableCursor;
    [SerializeField] RSE_DisableCursor DisableCursor;

    private void OnEnable()
    {
        EnableCursor.TriggerEvent += ShowCursor;
        EnableCursor.TriggerEvent += UnlockCursor;
        DisableCursor.TriggerEvent += HideCursor;
        DisableCursor.TriggerEvent += LockCursor;
    }

    private void OnDisable()
    {
        EnableCursor.TriggerEvent -= ShowCursor;
        EnableCursor.TriggerEvent -= UnlockCursor;
        DisableCursor.TriggerEvent -= HideCursor;
        DisableCursor.TriggerEvent -= LockCursor;
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}