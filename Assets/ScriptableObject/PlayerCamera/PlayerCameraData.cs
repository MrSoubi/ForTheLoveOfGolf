using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerControllerData", menuName = "SCO/PlayerControllerData")]
public class PlayerCameraData : ScriptableObject
{
    public float distance = 5f;
    public float focusRadius = 5f;
    public float focusCentering = 0.5f;
    public float rotationSpeed = 90f;
    public float minVerticalAngle = -45f;
    public float maxVerticalAngle = 45f;
    public float alignDelay = 5f;
    public float alignSmoothRange = 45f;

    public LayerMask obstructionMask = -1;

    public Vector2 orbitAngles = new Vector2(45f, 0f);
}
