using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBoost : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform player;
    [SerializeField] private Transform playerCam;

    private Rigidbody rb;
    private PlayerController pc;

    [Header("Boosting")]
    [SerializeField] private float boostDuration;

    private float boostIntensity;

    [Header("Cooldown")]
    public float dashCd;
    private float dashCdTimer;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pc = GetComponent<PlayerController>();
    }

    private void Update()
    {
        if(dashCdTimer > 0)
            dashCdTimer -= Time.deltaTime;
    }

    public void Boost(float intensity, Vector3 direction)
    {
        if (dashCdTimer > 0) return;
        else dashCdTimer = dashCd;

        pc.boosting = true;

        Vector3 forceToApply = direction * intensity;

        delayedForceToApply = forceToApply;
        Invoke(nameof(DelayedBoostForce), 0.025f);

        Invoke(nameof(ResetBoost), boostDuration);
    }

    private Vector3 delayedForceToApply;

    private void DelayedBoostForce()
    {
        rb.AddForce(delayedForceToApply, ForceMode.Impulse);
    }

    private void ResetBoost()
    {
        pc.boosting = false;
    }
}
