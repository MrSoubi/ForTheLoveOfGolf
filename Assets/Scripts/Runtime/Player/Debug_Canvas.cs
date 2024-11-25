using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DebugCanvas : MonoBehaviour
{
    public TMP_Text TMP_Velocity, TMP_SpeedLimit;
    public GameObject sphere;

    PlayerController pc;

    private void Start()
    {
        pc = sphere.GetComponent<PlayerController>();
    }

    void Update()
    {
        TMP_Velocity.text = pc.GetVelocity().magnitude.ToString();
        TMP_SpeedLimit.text = pc.GetCurrentSpeedLimit().ToString();
    }
}
