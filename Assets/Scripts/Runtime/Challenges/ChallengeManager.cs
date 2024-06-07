using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("__DEBUG__")]
    public int challengeQuantity;
    public int challengeCollected;

    public List<TimeChallenge> challenge = new List<TimeChallenge>();

    public Action onStartedChallenge;

    public TimeChallenge currentChallenge;

    public static ChallengeManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }
}
