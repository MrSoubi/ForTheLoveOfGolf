using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class PannelCollectible : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textCollectibles;
    [SerializeField] private GameObject holeGo;
    [SerializeField] private VisualEffect particleEffect;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private GameObject pannelGo;

    [Header("Settings")]
    [SerializeField] private float timeAnimation;
    [SerializeField] private int nbCollectibles;

    private void Start()
    {
        textCollectibles.text = 0 + "/" + nbCollectibles;

        if (CoinManager.instance) StartCoroutine(Utils.Delay(() => CoinManager.instance.onCollectedCoin += UpdatePannel, 0.05f));

        StartCoroutine(Utils.Delay(() => HoleManager.instance.AddHole(pannelGo.GetComponent<Hole>()), .001f));
        holeGo.SetActive(false);
    }

    /// <summary>
    /// Met à jour l'affichage sur le panneau
    /// </summary>
    private void UpdatePannel()
    {
        textCollectibles.text = CoinManager.instance.coinCollected + "/" + nbCollectibles;

        if (CoinManager.instance.coinCollected >= nbCollectibles)
        {
            if (CoinManager.instance) CoinManager.instance.onCollectedCoin -= UpdatePannel;

            if(sfx != null) sfx.Play();

            if(particleEffect != null) particleEffect.Play();

            StartCoroutine(Utils.Delay(() =>
            {
                Destroy(transform.GetChild(0).gameObject);
                holeGo.SetActive(true);
            }, timeAnimation * 0.7f));
        }
    }
}
