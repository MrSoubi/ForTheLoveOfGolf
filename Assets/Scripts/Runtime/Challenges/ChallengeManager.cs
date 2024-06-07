using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [HideInInspector]
    public List<TimeChallenge> challenge = new List<TimeChallenge>();

    public TimeChallenge currentChallenge;

    public static ChallengeManager instance;

    private void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }
}
