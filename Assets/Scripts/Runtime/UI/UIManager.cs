using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Collectibles")]
    [SerializeField] private TextMeshProUGUI coinValueText;
    [SerializeField] private TextMeshProUGUI holeValueText;
    [Header("TimeChallenge")]
    [SerializeField] private GameObject timeChallenge;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerCoinValueText;
    [Header("Shoot Icon")]
    [SerializeField] private Image shootIcon;
    [SerializeField] private Sprite shoot;
    [SerializeField] private Sprite shootTransparent;
    [Header("Other")]
    [SerializeField] private Animator circularFadeAnim;
    public Animator timerAnim;
    [Header("__DEBUG__")]

    public static UIManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    private void Start()
    {
        StartCoroutine(Utils.Delay(LinkEvent, 0.05f));

        StartCoroutine(Utils.Delay(() => UpdateCoinText(), .002f));
        StartCoroutine(Utils.Delay(() => UpdateHoleText(), .002f));
    }

    private void LinkEvent() {
        if (CoinManager.instance) CoinManager.instance.onCollectedCoin += UpdateCoinText;
        if (HoleManager.instance) HoleManager.instance.onCollectedHole += UpdateHoleText;
    }

    public void TimerSetActive(bool state)
    {
        timeChallenge.SetActive(state);
    }

    public void UpdateCoinText()
    {
        coinValueText.text = CoinManager.instance.coinCollected + "/" + CoinManager.instance.coinQuantity;
    }

    public void UpdateHoleText()
    {
        holeValueText.text = HoleManager.instance.holeCollected + "/" + HoleManager.instance.holeQuantity;
    }

    public void UpdateTimerText(string value)
    {
        timerText.text = value + "s";
    }

    public void UpdateTimerCoinText(string value, string maxValue)
    {
        timerCoinValueText.text = value + "/" + maxValue;
    }

    public void FadeIn()
    {
        circularFadeAnim.SetTrigger("FadeIn");
    }
    public void FadeOut()
    {
        circularFadeAnim.SetTrigger("FadeOut");
    }

    private IEnumerator HidUIAnim()
    {
        timerAnim.SetTrigger("Shake");

        yield return new WaitForSeconds(1f);
        timerAnim.SetBool("Show", false);
    }

    public void ChallengeInterface(bool val)
    {
        if(val)
        {
            timerAnim.SetBool("Show", true);
        }
        else
        {
            StartCoroutine(HidUIAnim());
        }
    }

    public void ShootUse(bool val)
    {
        if(val)
        {
            shootIcon.sprite = shootTransparent;
        }
        else
        {
            shootIcon.sprite = shoot;
        }
    }
}