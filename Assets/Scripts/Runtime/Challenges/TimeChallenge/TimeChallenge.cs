using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;

public class TimeChallenge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject triggerBox;
    [SerializeField] private TimeChallengeCoin[] coinList;

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
        CollectCoin();

        CoinSetActive(false);
        RewardSetActive(false);
        TriggerBoxSetActive(true);
    }

    private void CollectCoin()
    {
        for(int i = 0; i < coinList.Length; i++)
        {
            //coinList[i].onCollected;
        }
    }

    private void CoinSetActive(bool state)
    {
        for(int i = 0; i < coinList.Count(); i++)
        {
            coinList[i].gameObject.SetActive(state);
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

        TriggerBoxSetActive(false);
        CoinSetActive(true);

        StartCoroutine(Timer());
    }

    IEnumerator Timer()
    {
        while(timeToComplete > 0)
        {
            timeToComplete -= Time.deltaTime;
            yield return null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !started)
        {
            StartChallenge();
        }
    }
}