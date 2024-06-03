using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerData", menuName = "SCO/PlayerControllerData")]
public class PlayerControllerData : ScriptableObject
{
    public float maxSpeed = 10f;

    public float maxAcceleration = 10f;
    public float maxAirAcceleration = 1f;

    public float shootHeight = 2f;
    public int maxShoots = 1;

    public float maxGroundAngle = 25f;
    public float maxSnapSpeed = 100f;

    public float probeDistance = 1f;

<<<<<<< HEAD
    public List<float> speedLimits = new List<float>();
=======
    public List<float> speedLimits;
>>>>>>> Save
    public float speedLimitMargin;

    public Material rollingMaterial;
    public Material aimingMaterial;

    public float shootingAngle;

    public AnimationCurve shootCurve = AnimationCurve.Linear(0,0,1,1);
}
