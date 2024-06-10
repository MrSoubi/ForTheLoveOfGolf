using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("References")]
    public ParticleSystem stars;
    public GameObject mesh;

    [Header("Settings")]
    public int value;

    private void Start()
    {
        StartCoroutine(Utils.Delay(() => CoinManager.instance.AddCoin(this), .001f));
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player")) CoinManager.instance.CollectCoin(this);
    }
}