using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge : MonoBehaviour
{
    [Header("Challenge conditions")]
    [Tooltip("Coin require to finish le challenge")]
    [SerializeField] int coinsToGet; 
    int currentCoinGet;

    [Tooltip("Time of the challenge")]
    [SerializeField] float maxTime;
    float timer;

    bool isAlreadyFinish = false;

    ChallengeCollectible[] currentCollectibles;

    [Header("Challenge rewards")]
    [Tooltip("Objects that will appear if you win the challenge")] 
    [SerializeField] GameObject[] challengeRewards;

    Coroutine timerCoroutine;

    private void Awake()
    {
        currentCollectibles = GetComponentsInChildren<ChallengeCollectible>();
        for (int i = 0; i < currentCollectibles.Length; i++)
        {
            currentCollectibles[i].challengeReference = this;
        }
    }

    private void Start()
    {
        SetActiveCollectible(false);


        for (int i = 0; i < challengeRewards.Length; i++)
        {
            challengeRewards[i].SetActive(false);
        }
    }

    public void AddCoin(int coin)
    {
        currentCoinGet += coin;
        ChallengeManager.instance.SetCollectibleCount(currentCoinGet, coinsToGet);
        if(coinsToGet <= currentCoinGet)
        {
            isAlreadyFinish = true;

            for (int i = 0; i < challengeRewards.Length; i++)
            {
                challengeRewards[i].SetActive(true);
                challengeRewards[i].transform.SetParent(null);
            }
            EndChallenge();
        }
    }

    void SetChallenge()
    {
        currentCoinGet = 0;
        timer = maxTime;

        ChallengeManager.instance.StartNewChallenge(this, currentCoinGet, coinsToGet);
        
        SetActiveCollectible(true);
    }
    void StartChallenge()
    {
        timerCoroutine = StartCoroutine(ChallengeTimer());
    }
    void EndChallenge()
    {
        ChallengeManager.instance.StopCurrentChallenge();

        SetActiveCollectible(false);
        StopCoroutine(timerCoroutine);

        if (isAlreadyFinish) Destroy(gameObject);
    }

    IEnumerator ChallengeTimer()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            ChallengeManager.instance.timerTxt.text = timer.ToString("F2");
            yield return null;
        }

        EndChallenge();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !isAlreadyFinish)
        {
            if(ChallengeManager.instance.isDoingChallenge) ChallengeManager.instance.StopCurrentChallenge();
            SetChallenge();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player") && !isAlreadyFinish)
        {
            StartChallenge();
        }
    }

    public void SetActiveCollectible(bool active)
    {
        for (int i = 0; i < currentCollectibles.Length; i++)
        {
            currentCollectibles[i].gameObject.SetActive(active);
        }
    }
}