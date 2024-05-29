using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [HideInInspector]
    public CollectibleManager collectibleManager;

    [Header("Door")]
    public Door door;

    [Header("Animation")]
    public int animeDuration;

    private Vector3 position;

    private void Start()
    {
        position = transform.position;
    }

    /// <summary>
    /// Quand le joueur est détecter, il ouvre la porte uniquement si l'objectif est bon
    /// </summary>
    /// <param name="other">le collider qui entre en contact</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOMove(new Vector3(position.x, position.y - 0.3f, position.z), animeDuration);

            if (!door.open)
            {
                if(door.collectibleNeed && collectibleManager.collectibleCount >= door.collectibleQuantity)
                {
                    if(door.holeNeed && collectibleManager.holeCount >= door.holeQuantity)
                    {
                        door.OpenDoor();
                    }
                    else if(!door.holeNeed)
                    {
                        door.OpenDoor();
                    }
                }
                else if(door.holeNeed && collectibleManager.holeCount >= door.holeQuantity)
                {
                    if (door.collectibleNeed && collectibleManager.collectibleCount >= door.collectibleQuantity)
                    {
                        door.OpenDoor();
                    }
                    else if (!door.collectibleNeed)
                    {
                        door.OpenDoor();
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            transform.DOMove(new Vector3(position.x, position.y, position.z), animeDuration);
        }
    }
}
