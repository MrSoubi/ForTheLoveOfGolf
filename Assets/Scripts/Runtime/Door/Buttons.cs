using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [HideInInspector]
    public CollectibleManager collectibleManager;

    [Header("Door")]
    public Door door;
    public TextMeshProUGUI textCollectible;
    public TextMeshProUGUI textHole;

    [Header("Animation")]
    public int animeDuration;

    private Vector3 position;

    private void Start()
    {
        position = transform.position;

        if(door.collectibleNeed)
        {
            textCollectible.text = collectibleManager.collectibleCount.ToString() + "/" + door.collectibleQuantity.ToString() + " Coins";
            StartCoroutine(Utils.Delay(() => CollectibleManager.instance.onCollectedCoin += UpdatePannelCoin, 0.05f));
        }
        else
        {
            textCollectible.text = "";
        }

        if (door.holeNeed)
        {
            textHole.text = collectibleManager.holeCount.ToString() + "/" + door.holeQuantity.ToString() + " Holes";
            StartCoroutine(Utils.Delay(() => CollectibleManager.instance.onCollectedHole += UpdatePannelHole, 0.05f));
        }
        else
        {
            textHole.text = "";
        }
    }

    private void UpdatePannelCoin(int nbCollected)
    {
        if (!door.collectibleNeed) return;
        textCollectible.text = nbCollected + "/" + door.collectibleQuantity.ToString() + " Coins";
    }

    private void UpdatePannelHole(int nbCollected)
    {
        if (!door.holeNeed) return;
        textHole.text = nbCollected + "/" + door.holeQuantity.ToString() + " Holes";
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
