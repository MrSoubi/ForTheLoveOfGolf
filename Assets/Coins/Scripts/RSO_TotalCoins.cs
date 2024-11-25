using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Interfaces;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSO_TotalCoins", menuName = "Data/RSO/TotalCoins")]
public class RSO_TotalCoins : ScriptableObject
{
    public Action<int> onValueChanged;

    [ShowInInspector]
    private int _value;

    public int Value
    {
        get => _value;
        set
        {
            if (_value == value) return;

            _value = value;
            onValueChanged?.Invoke(_value);
        }
    }

    [Button]
    private void Add()
    {
        Value++;
    }

    [Button]
    private void Remove()
    {
        if (Value > 0) Value--;
    }

}