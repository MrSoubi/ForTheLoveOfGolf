using Sirenix.OdinInspector;
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "RSE_StandardGameEvent", menuName = "Data/RSE/StandardGameEvent")]
public class RSE_StandardGameEvent : ScriptableObject
{
    public Action TriggerEvent;
}
