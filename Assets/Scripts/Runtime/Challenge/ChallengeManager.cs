using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    public Animator timerAnim;
    public TMP_Text timerTxt;

    public Animator collectibleAnim;
    public TMP_Text collectibleCountTxt;

    public bool isDoingChallenge = false;
    [HideInInspector] public Challenge currentChallenge;

    public static ChallengeManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StopCurrentChallenge()
    {
        StartCoroutine(HidUIAnim());
        isDoingChallenge = false;
        currentChallenge.SetActiveCollectible(false);
        currentChallenge.StopAllCoroutines();
        currentChallenge = null;
    }

    public void StartNewChallenge(Challenge challenge, int currentCoinGet, int coinsToGet)
    {
        isDoingChallenge = true;
        currentChallenge = challenge;

        SetCollectibleCount(currentCoinGet, coinsToGet);

        timerTxt.text = "0.00";
        timerAnim.SetBool("Show", true);
        collectibleAnim.SetBool("Show", true);
        CollectibleManager.instance.HidCollectibleCount();
    }

    public void SetCollectibleCount(int current, int max)
    {
        collectibleCountTxt.text = current + " / " + max;
    }
    IEnumerator HidUIAnim()
    {
        timerAnim.SetTrigger("Shake");

        yield return new WaitForSeconds(2f);
        timerAnim.SetBool("Show", false);
        collectibleAnim.SetBool("Show", false);
        CollectibleManager.instance.ShowCollectibleCount();
    }
}