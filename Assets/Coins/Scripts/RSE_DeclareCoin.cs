using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_DeclareCoin", menuName = "Data/RSE/DeclareCoin")]
public class RSE_DeclareCoin : ScriptableObject
{
    public Action<int> TriggerEvent;

    [Button]
    private void FireEvent(int amount)
    {
        TriggerEvent?.Invoke(amount);
    }
}