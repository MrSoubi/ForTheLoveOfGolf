using Sirenix.OdinInspector;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Output")]
    public RSE_CollectCoin collectCoin;
    public RSE_DeclareCoin declareCoin;

    [Header("References")]
    [SerializeField] private AudioSource sfx;
    [SerializeField] private ParticleSystem stars;

    [Header("Settings")]
    [SerializeField] private int value;

    public void Start()
    {
        declareCoin.TriggerEvent?.Invoke(value);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (sfx != null) sfx.Play();

            if (stars != null)
            {
                ParticleSystem particle = Instantiate(stars, transform.position, transform.rotation);
                particle.transform.localScale = transform.localScale;
            }

            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        collectCoin.TriggerEvent?.Invoke(value);
    }

    [Button]
    private void Collect()
    {
        Destroy(gameObject);
    }
}