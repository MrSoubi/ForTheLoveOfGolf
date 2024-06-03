using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public TMP_Text TMP_Velocity, TMP_SpeedLimit;
    public GameObject sphere;

    PC_MovingSphere pc;

    private void Start()
    {
        pc = sphere.GetComponent<PC_MovingSphere>();
    }

    void Update()
    {
        TMP_Velocity.text = pc.GetVelocity().magnitude.ToString();
        TMP_SpeedLimit.text = pc.GetCurrentSpeedLimit().ToString();
    }
}
