using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Rotor : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, Time.deltaTime * speed, 0);
    }

    private void OnDrawGizmos()
    {
        if(transform.childCount != 0)
        {
            GameObject go = transform.GetChild(0).gameObject;
            Gizmos.DrawLine(transform.position, go.transform.position);

            Handles.DrawWireArc(transform.position, go.transform.up, go.transform.forward, 360, .5f);

            Vector3 centerPos = transform.position + transform.up * go.transform.localPosition.y;
            Handles.DrawWireArc(centerPos, go.transform.up, go.transform.forward, 360, Vector3.Distance(centerPos, go.transform.position));
        }
    }
}