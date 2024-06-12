using System.Collections;
using UnityEngine;

public class GiveShootCharge : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem stars;

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

            if (SoundManager.instance != null && SoundManager.instance.sfxShootCharger != null) SoundManager.instance.sfxShootCharger.Play();

            if (stars != null)
            {
                ParticleSystem particle = Instantiate(stars, transform.position, transform.rotation);
                particle.transform.localScale = transform.localScale;
            }

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
