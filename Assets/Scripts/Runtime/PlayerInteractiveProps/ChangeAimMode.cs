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
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            //DisableAimMode
            //EnableAimMode
        }
    }
}
