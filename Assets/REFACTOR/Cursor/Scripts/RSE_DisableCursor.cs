using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RSE_DisableCursor", menuName = "Data/RSE/DisableCursor")]
public class RSE_DisableCursor : ScriptableObject
{
    public Action TriggerEvent;

    [Button]
    private void FireEvent()
    {
        TriggerEvent?.Invoke();
    }
}
