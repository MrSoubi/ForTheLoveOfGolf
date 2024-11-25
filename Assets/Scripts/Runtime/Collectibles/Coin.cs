using UnityEngine;

public class Coin : MonoBehaviour
{
    public RSO_TotalCoins totalCoins;
    public RSO_CollectedCoins collectedCoins;
    public RSE_CollectCoin collectCoin;
    public RSE_DeclareCoin declareCoin;


    [SerializeField] private AudioSource sfx;
    [Header("References")]
    public ParticleSystem stars;
    public GameObject mesh;

    [Header("Settings")]
    public int value;

    private void Start()
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
}