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
    public Challenge currentChallenge;

    public static ChallengeManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void StopCurrentChallenge()
    {
        isDoingChallenge = false;
        currentChallenge.StopAllCoroutines();
        currentChallenge = null;
    }

    public void SetCollectibleCount(int current, int max)
    {
        collectibleCountTxt.text = current + " / " + max;
    }
}