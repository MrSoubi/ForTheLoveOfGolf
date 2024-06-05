using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buttons : MonoBehaviour
{
    [Header("Door")]
    [HideInInspector]
    public Door door;
    [HideInInspector]
    public TextMeshProUGUI textCoin;
    [HideInInspector]
    public TextMeshProUGUI textHole;

    [Header("Animation")]
    public int animeDuration;

    private Vector3 position;

    [HideInInspector]
    public AudioSource sfx;

    private void Start()
    {
        position = transform.position;

        if(door.collectibleNeed)
        {
            textCoin.text = CoinManager.instance.coinCollected.ToString() + "/" + door.collectibleQuantity.ToString() + " Coins";
            StartCoroutine(Utils.Delay(() => CoinManager.instance.onCollectedCoin += UpdatePannelCoin, 0.05f));
        }
        else
        {
            textCoin.text = "";
        }

        if (door.holeNeed)
        {
            textHole.text = HoleManager.instance.holeCompleted.ToString() + "/" + door.holeQuantity.ToString() + " Holes";
            StartCoroutine(Utils.Delay(() => HoleManager.instance.onCollectedHole += UpdatePannelHole, 0.05f));
        }
        else
        {
            textHole.text = "";
        }
    }

    private void UpdatePannelCoin(int nbCollected)
    {
        if (!door.collectibleNeed) return;
        textCoin.text = nbCollected + "/" + door.collectibleQuantity.ToString() + " Coins";
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
            sfx.Play();
            transform.DOMove(new Vector3(position.x, position.y - 0.3f, position.z), animeDuration);

            if (!door.open)
            {
                if(door.collectibleNeed && CoinManager.instance.coinCollected >= door.collectibleQuantity)
                {
                    if(door.holeNeed && HoleManager.instance.holeCompleted >= door.holeQuantity)
                    {
                        door.OpenDoor();
                    }
                    else if(!door.holeNeed)
                    {
                        door.OpenDoor();
                    }
                }
                else if(door.holeNeed && HoleManager.instance.holeCompleted >= door.holeQuantity)
                {
                    if (door.collectibleNeed && CoinManager.instance.coinCollected >= door.collectibleQuantity)
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
            sfx.Play();
            transform.DOMove(new Vector3(position.x, position.y, position.z), animeDuration);
        }
    }
}
