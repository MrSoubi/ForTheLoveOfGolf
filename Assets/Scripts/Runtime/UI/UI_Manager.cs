using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public RSO_CollectedCoins collectedCoins;
    public RSO_TotalCoins totalCoins;
    public RSO_CompletedHoles completedHoles;
    public RSO_TotalHoles totalHoles;

    [Header("Ref Collectibles")]
    [SerializeField] private TextMeshProUGUI coinValueText;
    [SerializeField] private TextMeshProUGUI holeValueText;

    [Header("Ref TimeChallenge")]
    [SerializeField] private GameObject timeChallenge;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI timerCoinValueText;

    [Header("Ref Animations")]
    public GameObject pannelPipe;
    [SerializeField] private Animator circularFadeAnim;
    [SerializeField] private Animator challengeAnim;

    public static UIManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    private void OnEnable()
    {
        collectedCoins.onValueChanged += UpdateCollectedCoins;
        completedHoles.onValueChanged += UpdateCompletedHoles;

        totalCoins.onValueChanged += UpdateTotalCoins;
        totalHoles.onValueChanged += UpdateTotalHoles;
    }

    private void Start()
    {
        UpdateCoinText();
        UpdateHoleText();
    }

    private void OnDisable()
    {
        collectedCoins.onValueChanged -= UpdateCollectedCoins;
        completedHoles.onValueChanged -= UpdateCompletedHoles;

        totalCoins.onValueChanged -= UpdateTotalCoins;
        totalHoles.onValueChanged -= UpdateTotalHoles;
    }

    private void UpdateCollectedCoins(int _) => UpdateCoinText();
    private void UpdateTotalCoins(int _) => UpdateCoinText();
    private void UpdateCompletedHoles(int _) => UpdateHoleText();
    private void UpdateTotalHoles(int _) => UpdateHoleText();


    public void UpdateCoinText()
    {
        coinValueText.text = collectedCoins.Value.ToString() + "/" + totalCoins.Value.ToString();
    }

    public void UpdateHoleText()
    {
        holeValueText.text = completedHoles.Value.ToString() + "/" + totalHoles.Value.ToString();
    }

    public void UpdateTimerText(string value)
    {
        timerText.text = value + "s";
    }

    public void UpdateTimerCoinText(string value, string maxValue)
    {
        timerCoinValueText.text = value + "/" + maxValue;
    }

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
    /// Met à jour l'opacité de l'icone de tire de la balle
    /// </summary>
    /// <param name="transparent"></param>
    public void ShootInterface(bool transparent)
    { 
    }
}