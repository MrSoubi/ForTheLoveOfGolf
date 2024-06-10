using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("Ref Collectibles")]
    [SerializeField] private TextMeshProUGUI coinValueText;
    [SerializeField] private TextMeshProUGUI holeValueText;

    [Header("Ref TimeChallenge")]
    [SerializeField] private GameObject timeChallenge;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerCoinValueText;

    [Header("Ref Shoot Icon")]
    [SerializeField] private Image shootIcon;
    [SerializeField] private Sprite shoot;
    [SerializeField] private Sprite shootTransparent;

    [Header("Ref Animations")]
    [SerializeField] private Animator circularFadeAnim;
    [SerializeField] private Animator challengeAnim;

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

    /// <summary>
    /// Met � jour les affichages � chaque fois que pi�ce ou un drapeau est collect�
    /// </summary>
    private void LinkEvent() 
    {
        if (CoinManager.instance) CoinManager.instance.onCollectedCoin += UpdateCoinText;
        if (HoleManager.instance) HoleManager.instance.onCollectedHole += UpdateHoleText;
    }

    /// <summary>
    /// Met � jour la valeur de pi�ces collecter sur le nombre max
    /// </summary>
    public void UpdateCoinText()
    {
        coinValueText.text = CoinManager.instance.coinCollected + "/" + CoinManager.instance.coinQuantity;
    }

    /// <summary>
    /// Met � jour la valeur de troue collecter sur le nombre max
    /// </summary>
    public void UpdateHoleText()
    {
        holeValueText.text = HoleManager.instance.holeCollected + "/" + HoleManager.instance.holeQuantity;
    }

    /// <summary>
    /// Met � jour le temps
    /// </summary>
    public void UpdateTimerText(string value)
    {
        timerText.text = value + "s";
    }

    /// <summary>
    /// Met � jour la valeur de pi�ces de challenge collecter sur le nombre max
    /// </summary>
    public void UpdateTimerCoinText(string value, string maxValue)
    {
        timerCoinValueText.text = value + "/" + maxValue;
    }

    /// <summary>
    /// Tremble l'interface et la cache
    /// </summary>
    private IEnumerator HideUIChallenge(bool shake)
    {
        if (shake)
        {
            challengeAnim.SetTrigger("Shake");

            yield return new WaitForSeconds(1f);
        }
        else yield return null;

        challengeAnim.SetBool("Show", false);
    }

    /// <summary>
    /// Affiche ou cache l'interface challenge
    /// </summary>
    /// <param name="val"></param>
    public void ChallengeInterface(bool val)
    {
        if (val) challengeAnim.SetBool("Show", true);
        else StartCoroutine(HideUIChallenge(true));
    }

    /// <summary>
    /// Active le timer du challenge
    /// </summary>
    /// <param name="state"></param>
    public void TimerSetActive(bool state)
    {
        timeChallenge.SetActive(state);
    }

    /// <summary>
    /// Affiche ou cache l'interface de transition du pipe
    /// </summary>
    /// <param name="show"></param>
    public void InterfacePipe(bool show)
    {
        if (show) circularFadeAnim.SetTrigger("FadeIn");
        else circularFadeAnim.SetTrigger("FadeOut");
    }

    /// <summary>
    /// Met � jour l'opacit� de l'icone de tire de la balle
    /// </summary>
    /// <param name="transparent"></param>
    public void ShootInterface(bool transparent)
    {
        if(transparent) shootIcon.sprite = shootTransparent;
        else shootIcon.sprite = shoot;
    }
}