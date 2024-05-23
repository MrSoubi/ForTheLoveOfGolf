using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Challenge : MonoBehaviour
{
    [Header("Challenge conditions")]
    public int coinsToGet;
    public int currentCoinGet;

    public float maxTime;
    float timer;

    [SerializeField] ChallengeCollectible[] currentCollectibles;

    [Header("Challenge rewards")]
    [SerializeField] GameObject[] challengeRewards;

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
    }

    public void AddCoin(int coin)
    {
        currentCoinGet += coin;
        ChallengeManager.instance.SetCollectibleCount(currentCoinGet, coinsToGet);
        if(coinsToGet <= currentCoinGet)
        {
            EndChallenge();

            for (int i = 0; i < challengeRewards.Length; i++)
            {
                challengeRewards[i].SetActive(true);
            }
        }
    }

    void SetChallenge()
    {
        ChallengeManager.instance.isDoingChallenge = true;
        ChallengeManager.instance.currentChallenge = this;

        ChallengeManager.instance.SetCollectibleCount(currentCoinGet, coinsToGet);

        timer = maxTime;
        ChallengeManager.instance.timerTxt.text = timer.ToString("F2");
        ChallengeManager.instance.timerAnim.SetBool("Show", true);
        ChallengeManager.instance.collectibleAnim.SetBool("Show", true);
        
        SetActiveCollectible(true);
    }
    void StartChallenge()
    {
        StartCoroutine(ChallengeTimer());
    }
    void EndChallenge()
    {
        SetActiveCollectible(false);
        StartCoroutine(HidTimerAnim());

        ChallengeManager.instance.timerAnim.SetTrigger("Shake");
    }

    IEnumerator HidTimerAnim()
    {
        yield return new WaitForSeconds(2f);
        ChallengeManager.instance.timerAnim.SetBool("Show", false);
        ChallengeManager.instance.collectibleAnim.SetBool("Show", false);

        ChallengeManager.instance.StopCurrentChallenge();
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
        if (other.transform.CompareTag("Player"))
        {
            if(ChallengeManager.instance.isDoingChallenge) ChallengeManager.instance.StopCurrentChallenge();
            SetChallenge();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            StartChallenge();
        }
    }

    void SetActiveCollectible(bool active)
    {
        for (int i = 0; i < currentCollectibles.Length; i++)
        {
            currentCollectibles[i].gameObject.SetActive(active);
        }
    }
}