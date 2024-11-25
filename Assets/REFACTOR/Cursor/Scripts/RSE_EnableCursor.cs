using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RSE_EnableCursor", menuName = "Data/RSE/EnableCursor")]
public class RSE_EnableCursor : ScriptableObject
{
    public Action TriggerEvent;

    [Button]
    private void FireEvent()
    {
        TriggerEvent?.Invoke();
    }
}
