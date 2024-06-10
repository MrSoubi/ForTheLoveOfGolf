using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SCO_SoundRequest", menuName = "SCO/SoundRequest")]
public class SoundRequest : ScriptableObject
{
    [Header("References")]
    [Tooltip("Source of the sound")] public AudioClip audio;
    [Tooltip("Type of sound")] public SoundCategory category = SoundCategory.SFX;

    [Header("Sound properties")]
    [Tooltip("Sound loop")] public bool looping;
    [Tooltip("Volume of the sound")][Range(0f, 1f)] public float volume = 1f;
    [Tooltip("Priority playing sound: (base = 128, smaller is, more is priority)")][Range(0, 256)] public int priority = 128;
    [Tooltip("Does sound is playing in 3D world, 0 = flat, 1 = spatilized")][Range(0f,1f)] public float spatializeSound;

    [HideInInspector] public AudioSource audioSourceAssign;
}

public enum SoundCategory
{
    MUSIC,
    SFX,
    AMBIENT
}
