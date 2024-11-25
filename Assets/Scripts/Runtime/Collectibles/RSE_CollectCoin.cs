using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_CollectCoin", menuName = "Data/RSO/CollectCoin")]
public class RSE_CollectCoin : ScriptableObject
{
    public Action<int> TriggerEvent;

    [Button]
    private void FireEvent(int amount)
    {
        TriggerEvent?.Invoke(amount);
    }
}