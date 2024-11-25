using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.VFX;

public class TimeChallenge : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private CinemachineVirtualCamera cam;
    [SerializeField] private GameObject triggerBox;
    [SerializeField] private ParticleSystem stars;
    [SerializeField] private List<TimeChallengeCoin> coinList = new List<TimeChallengeCoin>();
    [SerializeField] private List<Vector3> coinPos = new List<Vector3>();
    [SerializeField] private VisualEffect particleEffect;
    [SerializeField] private AudioSource sfx;
    [SerializeField] private AudioSource sfxReward;

    [Header("Cinematique Settings")]
    [SerializeField] private float focusDuration;
    [SerializeField] private float delayBeforeActivation;

    [Header("Time")]
    [SerializeField] private float timeToComplete;

    [Header("Rewards")]
    [SerializeField] private GameObject rewardHole;

    [Header("__DEBUG__")]
    public Coroutine timer;
    public bool started;
    public bool completed;
    public int coinCollected;
    public PlayerController pc;

    private void Start()
    {
        for (int i = 0; i < coinList.Count(); i++)
        {
            coinPos.Add(coinList[i].transform.position);
        }

        CoinSetActive(false);
        StartCoroutine(Utils.Delay(() => RewardSetActive(false), .005f));
        TriggerBoxSetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player") && !started)
        {
            pc = other.GetComponent<PlayerController>();
            StartChallenge();
        }
    }

    /// <summary>
    /// Active ou désactive toutes les pièces du challenge
    /// </summary>
    /// <param name="state"></param>
    private void CoinSetActive(bool state)
    {
        for (int i = 0; i < coinList.Count(); i++)
        {
            coinList[i].gameObject.SetActive(state);
            coinList[i].challenge = this;
            coinList[i].isPickingUp = false;
        }
    }

    /// <summary>
    /// Affiche ou cache la porte du challenge
    /// </summary>
    /// <param name="state"></param>
    private void TriggerBoxSetActive(bool state)
    {
        triggerBox.SetActive(state);
    }

    /// <summary>
    /// Affiche ou cache le troue du challenge
    /// </summary>
    /// <param name="state"></param>
    private void RewardSetActive(bool state)
    {
        rewardHole.SetActive(state);
    }

    /// <summary>
    /// Met à jour le timer du challenge
    /// </summary>
    private IEnumerator Timer()
    {
        float timer = timeToComplete;

        while (timer > 0)
        {
            timer -= Time.deltaTime;

            if (UIManager.instance != null) UIManager.instance.UpdateTimerText(timer.ToString("F2"));

            yield return null;
        }

        timer = 0;

        ResetChallenge();
    }

    /// <summary>
    /// Commence le challenge
    /// </summary>
    private void StartChallenge()
    {
        started = true;

        for (int i = 0; i < coinList.Count(); i++)
        {
            coinList[i].transform.position = coinPos[i];
        }

        ChallengeManager.instance.currentChallenge = this;

        if (UIManager.instance != null) UIManager.instance.UpdateTimerCoinText(coinCollected.ToString(), coinList.Count.ToString());
        if (UIManager.instance != null) UIManager.instance.UpdateTimerText(timeToComplete.ToString());
        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(true);

        TriggerBoxSetActive(false);
        CoinSetActive(true);

        timer = StartCoroutine(Timer());
    }

    /// <summary>
    /// Finis le challenge
    /// </summary>
    private void EndChallenge()
    {
        ChallengeManager.instance.currentChallenge = null;

        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(false);

        TriggerBoxSetActive(false);
        CoinSetActive(false);

        pc.ToggleRoll(true);
        pc.SetDirection(Vector3.zero);
        pc.Block();

        cameraManager.ActivateCamera(cam);

        StopCoroutine(timer);

        StartCoroutine(Utils.Delay(() =>
        {
            rewardHole.SetActive(true);

            if (sfxReward != null) sfxReward.Play();

            if (particleEffect != null)
            {
                particleEffect.transform.localScale = rewardHole.transform.localScale * 4;
                particleEffect.Play();
            }
        }, delayBeforeActivation));
        StartCoroutine(Utils.Delay(() =>
        {
            cameraManager.DeActivateCurrentCamera();
            pc.UnBlock(true);
        }, focusDuration));
    }

    /// <summary>
    /// Affiche la porte avec un délais lorsque le challenge est raté
    /// </summary>
    private IEnumerator TriggerBox()
    {
        yield return new WaitForSeconds(1);

        started = false;
        TriggerBoxSetActive(true);
    }

    /// <summary>
    /// Réinitalise le challenge
    /// </summary>
    public void ResetChallenge()
    {
        ChallengeManager.instance.currentChallenge = null;

        if (UIManager.instance != null) UIManager.instance.ChallengeInterface(false);

        coinCollected = 0;

        CoinSetActive(false);
        StartCoroutine(TriggerBox());
    }

    /// <summary>
    /// Si le challenge est en cour, le joueur respawn sur la porte du challenge
    /// </summary>
    /// <param name="player"></param>
    public void Respawn(GameObject player)
    {
        PlayerController tmp = player.GetComponent<PlayerController>();

        tmp.Block();
        player.transform.position = triggerBox.transform.position - new Vector3(0, 0.5f, 0);
        tmp.UnBlock(true);
    }

    /// <summary>
    /// Ajoute une pièce de challenge au compteur
    /// </summary>
    /// <param name="coin"></param>
    public void CollectCoin(TimeChallengeCoin coin)
    {
        coinCollected += coin.value;

        if (stars != null)
        {
            ParticleSystem particle = Instantiate(stars, coin.transform.position, coin.transform.rotation);
            particle.transform.localScale = coin.transform.localScale;
        }

        if (sfx != null) sfx.Play();

        if (UIManager.instance != null) UIManager.instance.UpdateTimerCoinText(coinCollected.ToString(), coinList.Count.ToString());

        if (coinCollected >= coinList.Count) EndChallenge();
        else coin.gameObject.SetActive(false);
    }
}