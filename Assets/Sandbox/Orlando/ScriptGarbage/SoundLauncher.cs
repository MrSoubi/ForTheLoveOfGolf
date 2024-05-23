using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLauncher : MonoBehaviour
{
    [SerializeField] SoundRequest soundRequest;

    void Start()
    {
        StartCoroutine(Utils.Delay(() => SoundManager.instance.PlaySound(soundRequest, transform.position),0.1f));
    }
}
