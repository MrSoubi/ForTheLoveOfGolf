using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeAimMode : MonoBehaviour
{
    private enum Type
    {
        EnableAimMode, DisableAimMode
    }

    [SerializeField] private Type type;
    private void OnTriggerEnter(Collider other)
    {
        PlayerController PC = other.GetComponent<PlayerController>();
        if (PC != null)
        {
            if (type == Type.EnableAimMode)
            {
                PC.ActivateShoot();
            }
            else
            {
                PC.DeactivateShoot();
            }
        }
    }
}
