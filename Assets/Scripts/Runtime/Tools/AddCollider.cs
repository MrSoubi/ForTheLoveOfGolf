using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AddCollider : MonoBehaviour
{
    [ContextMenuItem("Set Collider", "SetCollider")] public float Add_Collider;

    void SetCollider()
    {
        MeshRenderer[] tmp = GetComponentsInChildren<MeshRenderer>();
        for (int i = 0; i < tmp.Length; i++)
        {
            if (!tmp[i].TryGetComponent(out MeshCollider tmpCollid)) tmp[i].AddComponent<MeshCollider>();
        }
    }
}