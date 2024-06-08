using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class TimeChallenge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject triggerBox;
    [SerializeField] private ParticleSystem stars;
    [SerializeField] private  List<TimeChallengeCoin> coinList = new List<TimeChallengeCoin>();
    [SerializeField] private VisualEffect particleEffect;

    [Header("Settings")]
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

        if (UIManager.instance != null) UIManager.instance.UpdateTimerCoinText(coinCollected.ToString(), coinList.Count.ToString());

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
        if (UIManager.instance != null) UIManager.instance.UpdateTimerCoinText(coinCollected.ToString(), coinList.Count.ToString());
        if (UIManager.instance != null) UIManager.instance.UpdateTimerText(timeToComplete.ToString());
        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(true);

        TriggerBoxSetActive(false);
        CoinSetActive(true);

        StartCoroutine(Timer());
    }

    void EndChallenge()
    {
        ChallengeManager.instance.currentChallenge = null;
        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(false);

        StopAllCoroutines();

        TriggerBoxSetActive(false);
        CoinSetActive(false);   
        rewardHole.SetActive(true);
        if (particleEffect != null) particleEffect.Play();
    }

    private IEnumerator TriggerBox()
    {
        yield return new WaitForSeconds(1);

        started = false;
        TriggerBoxSetActive(true);
    }

    public void ResetChallenge()
    {
        ChallengeManager.instance.currentChallenge = null;
        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(false);

        coinCollected = 0;

        CoinSetActive(false);
        StartCoroutine(TriggerBox());
    }

    public void Respawn(GameObject player)
    {
        PC_MovingSphere tmp = player.GetComponent<PC_MovingSphere>();

        tmp.Block();
        player.transform.position = triggerBox.transform.position - new Vector3(0, 0.5f, 0);
        tmp.UnBlock(true);
    }

    IEnumerator Timer()
    {
        float timer = timeToComplete;
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            if (UIManager.instance != null) UIManager.instance.UpdateTimerText(timer.ToString("F2"));
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