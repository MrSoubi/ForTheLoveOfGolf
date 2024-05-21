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
            audioSourcesAvailable.Enqueue(PullAudioSource());
        }
    }

    public void StopSound(SoundRequest soundRequest)
    {
        if (soundRequest.audioSourceAssign == null) return;
        soundRequest.audioSourceAssign.Stop();
        FreeAudioSource(soundRequest.audioSourceAssign);
        soundRequest.audioSourceAssign = null;
    }

    public void PlaySound(SoundRequest soundRequest, Vector3 positionSound)
    {
        var audioSource = PickAudioSource();
        if (audioSource == null ||soundRequest == null) return;

        audioSource.clip = soundRequest.audio;
        audioSource.priority = soundRequest.priority;
        audioSource.volume = soundRequest.volume;
        audioSource.spatialBlend = soundRequest.spatializeSound;
        audioSource.loop = soundRequest.looping;
        audioSource.transform.position = positionSound;

        audioSource.Play();
        StartCoroutine(Utils.Delay(()=> FreeAudioSource(audioSource), soundRequest.audio.length));
    }

    private AudioSource PullAudioSource()
    {
        var go = new GameObject("AudioSource");
        go.transform.parent = transform;
        return go.AddComponent<AudioSource>();
    }

    private AudioSource PickAudioSource()
    {
        AudioSource audioSource = null;
        if (canExpendQueue) audioSource = PullAudioSource();
        else if (audioSourcesAvailable.Count > 0) audioSource =  audioSourcesAvailable.Dequeue();
        else Debug.LogWarning(message: "None AudioSource Available");
        return audioSource;
    }

    private void FreeAudioSource(AudioSource audioSource)
    {
        audioSource.volume = 1f;
        audioSource.priority = 128;
        audioSource.clip = null;
        audioSource.loop = false;
        audioSource.transform.position = Vector3.zero;

        audioSourcesAvailable.Enqueue(audioSource);
    }

}
