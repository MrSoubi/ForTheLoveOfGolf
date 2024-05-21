using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; private set; }

    private Queue audioSourcesAvailable = new Queue();
    private int quantityAudioSources = 10;

    public void Awake()
    {
        if (instance != null && this != instance)
        {
            Debug.LogWarning(message: $"SoundManager already exist in world, {gameObject.name} deleted!");
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    private void Start()
    {
        for(int i = 0; i < quantityAudioSources; ++i)
        {
            PullAudioSource();
        }
    }

    public void StopSound() { }

    public void PlaySound() { }

    private bool SoundIsPlaying() { return false; }

    private void PullAudioSource() 
    {

    }

}
