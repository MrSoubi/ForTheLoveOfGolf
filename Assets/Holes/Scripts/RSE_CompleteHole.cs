using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_CompleteHole", menuName = "Data/RSE/CompleteHole")]
public class RSE_CompleteHole : ScriptableObject
{
    public Action TriggerEvent;
}