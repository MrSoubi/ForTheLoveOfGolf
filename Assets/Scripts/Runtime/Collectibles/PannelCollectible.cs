using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PannelCollectible : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMP_Text textCollectibles;
    [SerializeField] private GameObject holePrefab;
    [SerializeField] private ParticleSystem particleEffect;
    [Space]
    [SerializeField][Min(0)] private int nbCollectibles;

    public void OnValidate()
    {
        if (!textCollectibles) return;
        textCollectibles.text = 0 + "/" + nbCollectibles;
    }

    private void Start()
    {
        StartCoroutine(Utils.Delay(() => CollectibleManager.instance.onCollected += UpdatePannel, 0.05f));
    }

    private void UpdatePannel(int nbCollected)
    {
        if (!textCollectibles) return;
        textCollectibles.text = nbCollected + "/" + nbCollectibles;
        if (nbCollected >= nbCollectibles)
        {
            if (CollectibleManager.instance) CollectibleManager.instance.onCollected -= UpdatePannel;
            //particleEffect.Play();
            //StartCoroutine(Utils.Delay(() => 
            //{
            //    Destroy(transform.GetChild(0).gameObject);
            //    Instantiate(holePrefab, transform);
            //}, particleEffect.totalTime * 0.6f));

            Destroy(transform.GetChild(0).gameObject);
            Instantiate(holePrefab, transform);
        }
    }
}