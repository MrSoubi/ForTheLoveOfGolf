using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Booster : MonoBehaviour
{
    [SerializeField, Min(0f)] private float acceleration = 10f;
    [SerializeField, Min(0f)] private float targetSpeedLimit = 10f;

    [SerializeField] private Vector3 teleportLocation;

    [SerializeField] private bool teleport;
    [SerializeField] private bool freeze;
    [SerializeField] private bool giveShoot;

    void OnTriggerEnter(Collider other)
    {
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            if (teleport)
            {
                PC.SetDirection(transform.forward);
                PC.Teleport(transform.position + teleportLocation);
            }
            if (freeze)
            {
                PC.Freeze();
            }
            if (giveShoot)
            {
                PC.AddShootCharges(1);
            }

            if(targetSpeedLimit >= PC.GetCurrentSpeedLimit())
            {
                PC.IncreaseSpeedLimit();
                PC.IncreaseVelocityToCurrentSpeedLimit();
            }
            else
            {
                PC.IncreaseSpeedLimitToMaximum();
                PC.IncreaseVelocityToCurrentSpeedLimit();
                //Set speed limit to target speed limit
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + teleportLocation, 0.1f);
    }
}
