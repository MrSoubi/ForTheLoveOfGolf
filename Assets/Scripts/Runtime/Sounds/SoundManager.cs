using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; private set; }
    private Queue<AudioSource> audioSourcesAvailable = new ();

    [Header("Properties")]
    [SerializeField] private int quantityAudioSources = 10;
    [SerializeField][Tooltip("Force to have more AudioSource, if all are used")] private bool canExpendQueue = false;

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

    public void StopSound(SoundRequest soundRequest) { }

    public void PlaySound(SoundRequest soundRequest) { }

    private bool SoundIsPlaying() { return false; }

    private void PullAudioSource() 
    {
        
    }

    private void FreeAudioSource(AudioSource audioSource)
    {
        audioSource.volume = 1f;
        audioSource.priority = 128;
        audioSource.clip = null;
        audioSource.loop = false;

        audioSourcesAvailable.Enqueue(audioSource);
    }

    private AudioSource PickAudioSource()
    {
        AudioSource audioSource = null;
        if (canExpendQueue) audioSource = new GameObject().AddComponent<AudioSource>();
        else if (audioSourcesAvailable.Count > 0) audioSource =  audioSourcesAvailable.Dequeue();
        else Debug.LogWarning(message: "None AudioSource Available");
        return audioSource;
    }

}
