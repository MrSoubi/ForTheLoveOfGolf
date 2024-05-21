using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Door : MonoBehaviour
{
    public int index;
    public int openType;

    public float duration;
    public float distance;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private Quaternion startRotation;
    private float elapsedTime;

    private bool open;
    private bool rotate;

    private void Start()
    {

        startPosition = transform.position;

        if(openType == 0)
        {
            endPosition = transform.position + new Vector3(0, distance, 0);
        }
        else if(openType == 1)
        {
            endPosition = transform.position + new Vector3(distance, 0, 0);
        }
        else
        {
            startRotation = transform.rotation;
        }
    }

    public void OpenDoor()
    {
        if(openType == 0)
        {
            OpenVerticaly();
        }
        else if(openType == 1)
        {
            OpenHorizontaly();
        }
        else
        {
            OpenPivot();
        }
    }

    public void OpenVerticaly()
    {
        elapsedTime = 0;
        open = true;
    }

    public void OpenHorizontaly()
    {
        elapsedTime = 0;
        open = true;
    }

    public void OpenPivot()
    {
        elapsedTime = 0;
        rotate = true;
    }

    private void Update()
    {
        if(open)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / duration;

            transform.position = Vector3.Lerp(startPosition, endPosition, percentageComplete);

            if(startPosition == endPosition)
            {
                open = false;
            }
        }

        // TO FINISH
        if(rotate)
        {
            elapsedTime += Time.deltaTime;
            float percentageComplete = elapsedTime / duration;

            transform.rotation = Quaternion.Lerp(startRotation, new Quaternion(0, 0, 90, 0), percentageComplete);

            if(percentageComplete == duration)
            {
                rotate = false;
            }
        }
    }
}
