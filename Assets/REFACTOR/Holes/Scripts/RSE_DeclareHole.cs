using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "RSE_DeclareHole", menuName = "Data/RSE/DeclareHole")]
public class RSE_DeclareHole : ScriptableObject
{
    public Action TriggerEvent;
}