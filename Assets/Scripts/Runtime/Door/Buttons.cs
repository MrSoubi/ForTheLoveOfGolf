using DG.Tweening;
using UnityEngine;

public class Buttons : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private Door door;
    [SerializeField] private AudioSource sfx;

    [Header("Setting")]
    [SerializeField] private int animeDuration;
    
    private Vector3 posButton;

    private void Start()
    {
        posButton = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sfx != null) sfx.Play();

            door?.TriggerOpen();
            transform.DOMove(new Vector3(posButton.x, posButton.y - 0.2f, posButton.z), animeDuration);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(sfx != null) sfx.Play();
            
            transform.DOMove(new Vector3(posButton.x, posButton.y, posButton.z), animeDuration);
        }
    }
}
