using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] private float threasholdSpeed;

    [Header("References")]
    [SerializeField] private GameObject particleDestroyed;
    [SerializeField] private Collider colliderMesh;

    private void Start() => EnableCollider(false);

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.transform.GetComponent<PlayerController>();
            var velocity = player.GetVelocity();

            if (velocity.magnitude >= threasholdSpeed)
            {
                if (SoundManager.instance != null && SoundManager.instance.sfx != null) SoundManager.instance.sfx.Play();

                if (particleDestroyed)
                {
                    GameObject particle = Instantiate(particleDestroyed, transform.position, Quaternion.identity);
                    particle.transform.localScale = transform.localScale;
                }

                gameObject.SetActive(false);
                return;
            }
        }
        EnableCollider(true);

    }
    private void OnTriggerExit(Collider other)
    {
        EnableCollider(false);
    }

    private void EnableCollider(bool active) => colliderMesh.enabled = active;

}
