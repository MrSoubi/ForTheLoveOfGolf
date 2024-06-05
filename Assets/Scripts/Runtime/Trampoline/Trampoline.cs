using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Trampoline : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField, Min(0f)] private float acceleration = 10f;
    [SerializeField, Min(0f)] private int targetSpeedLimit;
    [SerializeField] private Vector3 teleportPoint;
    [SerializeField] private bool teleport;
    [SerializeField] private bool freeze;
    [SerializeField] private bool giveShoot;
    [SerializeField] private int shootQuantityToGive = 1;

    void OnTriggerEnter(Collider other)
    {    
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            if(teleport) PC.Teleport(transform.position + teleportPoint);
            if(freeze) PC.Freeze();
            if(giveShoot) PC.AddShootCharges(1);

            PC.SetDirection(transform.up);

            Vector3 currentSpeed = PC.GetVelocity();
            if(PC.GetCurrentSpeedLimit() >= targetSpeedLimit)
            {
                PC.IncreaseSpeedLimit();
                PC.IncreaseVelocityToCurrentSpeedLimit();
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + teleportPoint, 0.1f);
    }
}
