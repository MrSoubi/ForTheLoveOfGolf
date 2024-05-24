using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [HideInInspector]
    public CollectibleManager collectibleManager;

    [Header("Door")]
    public Door door;

    /// <summary>
    /// Quand le joueur est détecter, il ouvre la porte uniquement si l'objectif est bon
    /// </summary>
    /// <param name="other">le collider qui entre en contact</param>
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (!door.open)
            {
                if (door.collectibleNeed && collectibleManager.collectibleValue >= door.collectibleQuantity)
                {
                    door.OpenDoor();
                }
                else if (door.holeNeed && collectibleManager.holeValue >= door.holeQuantity)
                {
                    door.OpenDoor();
                }
            }
        }
    }
}
