using System.Collections;
using UnityEngine;

public class Challenge : MonoBehaviour
{
    public GameObject door;

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
    [SerializeField] GameObject challengeRewards;

    Coroutine timerCoroutine;

    [HideInInspector] public bool active;
    [HideInInspector] public int index;

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

        if (coinsToGet == 0) coinsToGet = currentCollectibles.Length;
        challengeRewards.SetActive(false);
        SetActiveCollectible(false);
    }

    public void AddCoin(int coin)
    {
        currentCoinGet += coin;
        ChallengeManager.instance.SetCollectibleCount(currentCoinGet, coinsToGet);
        if(coinsToGet <= currentCoinGet)
        {
            isAlreadyFinish = true;
            
            EndChallenge();
        }
    }

    void SetChallenge()
    {
        currentCoinGet = 0;
        timer = maxTime;

        ChallengeManager.instance.StartNewChallenge(this, maxTime, currentCoinGet, coinsToGet);
        
        SetActiveCollectible(true);
    }
    void StartChallenge()
    {
        gameObject.GetComponent<BoxCollider>().enabled = false;
        door.GetComponent<MeshRenderer>().enabled = false;

        timerCoroutine = StartCoroutine(ChallengeTimer());
        active = true;
    }
    public void EndChallenge()
    {
        active = false;
        ChallengeManager.instance.StopCurrentChallenge();

        SetActiveCollectible(false);
        StopCoroutine(timerCoroutine);

        if (isAlreadyFinish)
        {
            challengeRewards.SetActive(true);
            SaveManager.challenges[index] = true;
            Destroy(gameObject);
        }

        gameObject.GetComponent<BoxCollider>().enabled = true;
        door.GetComponent<MeshRenderer>().enabled = true;
    }

    IEnumerator ChallengeTimer()
    {
        while(timer > 0)
        {
            timer -= Time.deltaTime;
            ChallengeManager.instance.SetTimer(timer);
            yield return null;
        }

        EndChallenge();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !isAlreadyFinish)
        {
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