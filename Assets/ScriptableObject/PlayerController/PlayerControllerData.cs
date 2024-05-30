using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerData", menuName = "SCO/PlayerControllerData")]
public class PlayerControllerData : ScriptableObject
{
    public float maxSpeed = 10f;
    public float maxAcceleration = 10f;
    public float maxAirAcceleration = 1f;
    public float maxGroundAngle = 25f;
    public float maxSnapSpeed = 100f;

    public float probeDistance = 1f;

    public LayerMask probeMask = -1;

    public Material normalMaterial = default;

    public float ballRadius = 0.5f;
    public float ballAlignSpeed = 180f;
    public float ballAirRotation = 0.5f;
}
