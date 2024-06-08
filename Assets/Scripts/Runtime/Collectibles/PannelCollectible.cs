using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.VFX;

public class PannelCollectible : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textCollectibles;
    [SerializeField] private GameObject holeGo;
    [SerializeField] private VisualEffect particleEffect;
    [SerializeField] private GameObject pannelGo;
    [SerializeField] private float timeAnimation;
    [Space]
    [SerializeField][Min(0)] private int nbCollectibles;

    public void OnValidate()
    {
        if (!textCollectibles) return;
        textCollectibles.text = 0 + "/" + nbCollectibles;
    }

    private void Start()
    {
        if(CoinManager.instance) StartCoroutine(Utils.Delay(() => CoinManager.instance.onCollectedCoin += UpdatePannel, 0.05f));
        StartCoroutine(Utils.Delay(() => HoleManager.instance.AddHole(pannelGo.GetComponent<Hole>()), .001f));
        holeGo.SetActive(false);
    }

    private void UpdatePannel()
    {
        if (!textCollectibles) return;
        textCollectibles.text = CoinManager.instance.coinCollected + "/" + nbCollectibles;
        if (CoinManager.instance.coinCollected >= nbCollectibles)
        {
            if (CoinManager.instance) CoinManager.instance.onCollectedCoin -= UpdatePannel;
            if (particleEffect != null) particleEffect.Play();
            StartCoroutine(Utils.Delay(() =>
            {
                Destroy(transform.GetChild(0).gameObject);
                holeGo.SetActive(true);
            }, timeAnimation * 0.7f));
        }
    }
}
