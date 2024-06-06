using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GiveShootCharge : MonoBehaviour
{
    [SerializeField] private int quantityToGive = 1;
    private void OnTriggerEnter(Collider other)
    {
        PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
        if (PC != null)
        {
            PC.AddShootCharges(quantityToGive);
        }
    }
}
