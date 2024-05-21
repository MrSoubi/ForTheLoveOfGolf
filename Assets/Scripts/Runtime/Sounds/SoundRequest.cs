using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_SoundRequest", menuName = "SCO/SoundRequest")]
public class SoundRequest : ScriptableObject
{
    [Header("References")]
    [Tooltip("Source of the sound")] public AudioClip audio;

    [Header("Sound properties")]
    [Tooltip("Sound loop")] public bool looping;
    [Tooltip("Volume of the sound")][Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Priority playing sound: (base = 128, smaller is, more is priority)")][Range(0, 256)] public int priority = 128;
}
