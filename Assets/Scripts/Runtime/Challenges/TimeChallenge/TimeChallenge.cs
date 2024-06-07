using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class TimeChallenge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject triggerBox;
    [SerializeField] private ParticleSystem stars;
    [SerializeField] private  List<TimeChallengeCoin> coinList = new List<TimeChallengeCoin>();

    [Header("Settings")]
    [SerializeField] private Vector3 respawnPoint;
    [SerializeField] private float timeToComplete;

    [Header("Rewards")]
    [SerializeField] private GameObject rewardHole;

    [Header("__DEBUG__")]
    public bool started;
    public bool completed;
    public int coinCollected;

    private void Start()
    {
        TimeChallengeCoin.onCollected += CollectCoin;

        CoinSetActive(false);
        StartCoroutine(Utils.Delay(() => RewardSetActive(false), .005f));
        TriggerBoxSetActive(true);
    }

    private void CollectCoin(TimeChallengeCoin coin)
    {
        coinCollected += coin.value;

        if (stars != null)
        {
            Instantiate(stars, coin.transform.position, coin.transform.rotation);
        }

        if (coinCollected >= coinList.Count) EndChallenge();
        else coin.gameObject.SetActive(false);
    }

    private void CoinSetActive(bool state)
    {
        for(int i = 0; i < coinList.Count(); i++)
        {
            coinList[i].gameObject.SetActive(state);
            coinList[i].isPickingUp = false;
        }
    }

    private void RewardSetActive(bool state)
    {
        rewardHole.SetActive(state);
    }

    private void TriggerBoxSetActive(bool state)
    {
        triggerBox.SetActive(state);
    }

    void StartChallenge()
    {
        started = true;
        ChallengeManager.instance.currentChallenge = this;

        TriggerBoxSetActive(false);
        CoinSetActive(true);

        StartCoroutine(Timer());
    }

    void EndChallenge()
    {
        started = false;
        ChallengeManager.instance.currentChallenge = null;

        StopAllCoroutines();

        TriggerBoxSetActive(false);
        CoinSetActive(false);
        rewardHole.SetActive(true);
    }

    public void ResetChallenge()
    {
        started = false;
        ChallengeManager.instance.currentChallenge = null;

        coinCollected = 0;

        TriggerBoxSetActive(true);
        CoinSetActive(false);
    }

    IEnumerator Timer()
    {
        float timer = timeToComplete;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            yield return null;
        }

        ResetChallenge();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !started)
        {
            StartChallenge();
        }
    }
}