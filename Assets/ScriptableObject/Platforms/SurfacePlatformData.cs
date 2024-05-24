using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlatformData", menuName = "SCO/PlatformData")]
public class SurfacePlatformData : ScriptableObject
{
    public PlatformType type;

    public int drag;
    public float moveSpeedMultiplier;
}

[System.Serializable]
public enum PlatformType
{
    Slow,
    Ice,
}