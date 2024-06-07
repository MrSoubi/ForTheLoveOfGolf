using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

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
    [SerializeField] private GameObject shootIcon;
    [Header("Other")]
    [SerializeField] private Animator circularFadeAnim;

    public static UIManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    public void TimerSetActive(bool state)
    {
        timeChallenge.SetActive(state);
    }

    public void UpdateCoinText(string value, string maxValue)
    {
        coinValueText.text = value + "/" + maxValue;
    }

    public void UpdateHoleText(string value, string maxValue)
    {
        holeValueText.text = value + "/" + maxValue;
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
    public  void FadeOut()
    {
        circularFadeAnim.SetTrigger("FadeOut");
    }
}