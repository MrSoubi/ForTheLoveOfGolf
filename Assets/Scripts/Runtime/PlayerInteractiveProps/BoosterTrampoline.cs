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
    [SerializeField] private Transform teleportLocation;

    [SerializeField] private bool teleport;
    [SerializeField] private bool freeze;
    [SerializeField] private bool giveShoot;

    [SerializeField] private AudioSource sfx;

    private void OnTriggerEnter(Collider other)
    {
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            if (teleport)
            {
                PC.Teleport(teleportLocation);
            }
            if (freeze)
            {
                PC.Freeze();
            }
            if (giveShoot)
            {
                PC.AddShootCharges(1);
            }

            if(targetSpeedLimit > PC.GetSpeedLimitIndex())
            {
                PC.SetSpeedLimit(targetSpeedLimit);
            }
            else
            {
                PC.IncreaseSpeedLimit();
            }

            PC.IncreaseVelocityToCurrentSpeedLimit();

            PC.SetDirection(type == Type.BOOSTER ? transform.forward : transform.up);

            if(sfx != null) sfx.Play();
        }
    }
}
