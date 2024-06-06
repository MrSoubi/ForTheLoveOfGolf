using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoosterTrampoline : MonoBehaviour
{
    private enum Type
    {
        BOOSTER,
        TRAMPOLINE
    }
    [SerializeField, Range(0,10)] private int targetSpeedLimit = 10;

    [SerializeField] private Type type;
    [SerializeField] private Vector3 teleportLocation;

    [SerializeField] private bool teleport;
    [SerializeField] private bool freeze;
    [SerializeField] private bool giveShoot;

    private void OnTriggerEnter(Collider other)
    {
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            if (teleport)
            {
                PC.SetDirection(type == Type.BOOSTER ? transform.forward : transform.up);
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
            }
            else
            {
                PC.IncreaseVelocityToCurrentSpeedLimit();
            }
            PC.IncreaseVelocityToCurrentSpeedLimit();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + teleportLocation, 0.1f);
    }
}
