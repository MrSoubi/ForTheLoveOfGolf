using System.Collections;
using UnityEngine;

public class GiveShootCharge : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem stars;
    [SerializeField] private AudioSource sfx;

    [Header("Settings")]
    [SerializeField] private int quantityToGive = 1;
    [SerializeField] private int timeReset = 3;

    [Header("__DEBUG__")]
    [SerializeField] private bool take;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !take)
        {
            take = true;
            StartCoroutine(ResetCoin());

            if(sfx != null) sfx.Play();

            if (stars != null) Instantiate(stars, transform.position, transform.rotation);

            PC_MovingSphere PC = other.GetComponent<PC_MovingSphere>();
            PC.AddShootCharges(quantityToGive);
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Réinitialise la pièce
    /// </summary>
    /// <returns></returns>
    private IEnumerator ResetCoin()
    {
        yield return new WaitForSeconds(timeReset);

        take = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
    }
}
