﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerData", menuName = "SCO/PlayerControllerData")]
public class PlayerControllerData : ScriptableObject
{
    //public float maxSpeed = 10f;

    public float
        shootingFactor = 0.5f;

    public float shootHeight = 2f;
    public int maxShoots = 1;

    public float shootingAngle;

    [Space]

    public float maxAcceleration = 10f;
    public float maxAirAcceleration = 1f;

    public AnimationCurve rotationCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public List<float> speedLimits = new List<float>();
    public float speedLimitMargin = 1f;

    [Space]

    public float maxGroundAngle = 25f;
    public float maxSnapSpeed = 100f;

    public float probeDistance = 1f;



    [Space]
    public Material rollingMaterial;
    public Material aimingMaterial;
}
