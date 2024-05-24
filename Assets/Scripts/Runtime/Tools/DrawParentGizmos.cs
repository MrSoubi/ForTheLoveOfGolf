using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class DrawParentGizmos : MonoBehaviour
{
    [SerializeField] private UnityEvent onCallParent;

    void OnDrawGizmosSelected()
    {
        onCallParent.Invoke();
    }
}
