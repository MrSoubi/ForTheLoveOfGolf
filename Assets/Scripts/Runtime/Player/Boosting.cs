using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boosting : MonoBehaviour
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
        if(Input.GetKeyDown(KeyCode.E))
        {
            Boost(5, new Vector3(1, 0, 0));
        }
    }

    private void Boost(float intensity, Vector3 direction)
    {
        Vector3 forceToApply = direction * intensity;

        rb.AddForce(forceToApply, ForceMode.Impulse);

        Invoke(nameof(ResetBoost), boostDuration);
    }

    private void ResetBoost()
    {

    }
}
