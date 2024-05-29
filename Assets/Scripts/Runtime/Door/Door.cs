using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UIElements;

public class Door : MonoBehaviour
{
    [HideInInspector]
    public bool openVertical;

    [Header("Animation")]
    public int animeDuration;

    [Header("State")]
    public bool open;

    [Header("Collectible Need")]
    public bool collectibleNeed;
    public int collectibleQuantity;

    [Header("Hole Need")]
    public bool holeNeed;
    public int holeQuantity;

    private void Start()
    {
        if (open)
        {
            OpenDoor();
        }
    }

    /// <summary>
    /// Ouvre la porte selon son type
    /// </summary>
    public void OpenDoor()
    {
        open = true;

        if(openVertical)
        {
            OpenVerticaly();
        }
        else
        {
            OpenPivot();
        }
    }

    /// <summary>
    /// Ouvre la porte verticalement vers le bas
    /// </summary>
    private void OpenVerticaly()
    {
        GameObject door = gameObject.transform.GetChild(0).gameObject;

        door.transform.DOMove(new Vector3(transform.position.x, transform.position.y - 4, transform.position.z), animeDuration);
    }

    /// <summary>
    /// Ouvre les 2 portes en les pivotants
    /// </summary>
    private void OpenPivot()
    {
        GameObject doorLeft = gameObject.transform.GetChild(0).gameObject;
        GameObject doorRight = gameObject.transform.GetChild(1).gameObject;

        doorLeft.transform.DORotate(new Vector3(transform.rotation.x, transform.rotation.y - 90, transform.rotation.z), animeDuration);
        doorRight.transform.DORotate(new Vector3(transform.rotation.x, transform.rotation.y + 90, transform.rotation.z), animeDuration);
    }
}
