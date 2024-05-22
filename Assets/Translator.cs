using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Travel : MonoBehaviour
{
    private GameObject subject; // le GO qui est translaté
    private GameObject start; // la position de départ
    private GameObject end; // la position d'arrivée

    public float frequency;

    void Start()
    {
        subject = transform.GetChild(0).gameObject;
        start = transform.GetChild(1).gameObject;
        end = transform.GetChild(2).gameObject;

        subject.transform.position = start.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        subject.transform.position = Vector3.Lerp(start.transform.position, end.transform.position, (1.0f + Mathf.Cos(Time.time * frequency * Mathf.PI)) / 2.0f);
    }
}