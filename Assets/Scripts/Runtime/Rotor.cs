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
        transform.Rotate(0, 0, Time.deltaTime * speed);
    }

    private void OnDrawGizmos()
    {
        if(transform.childCount != 0)
        {
            GameObject go = transform.GetChild(0).gameObject;
            Gizmos.DrawLine(transform.position, go.transform.position);
        }
    }
}