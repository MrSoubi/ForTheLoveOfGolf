using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance {  get; private set; }
    private Queue<AudioSource> audioSourcesAvailable = new ();
    public AudioSource sfx;
    public AudioSource sfxShootCharger;

    [Header("References")]
    [Tooltip("Don't touch! MUSIC = 0, SFX = 1, AMBIENT = 2")][SerializeField] private List<AudioMixerGroup> mixerGroups;

    [Header("Properties")]
    [SerializeField] private int quantityAudioSources = 10;
    [SerializeField][Tooltip("Force to have more AudioSource, if all are used")] private bool canExpendQueue = false;

    public void Awake()
    {
        instance = this.Singleton(instance, () => Destroy(gameObject));
    }

    private void Start()
    {
        for(int i = 0; i < quantityAudioSources; ++i)
        {
            audioSourcesAvailable.Enqueue(PullAudioSource());
        }
    }

    /// <summary>
    /// Stop le son définit
    /// </summary>
    /// <param name="soundRequest"></param>
    public void StopSound(SoundRequest soundRequest)
    {
        if (soundRequest.audioSourceAssign == null) return;

        soundRequest.audioSourceAssign.Stop();
        FreeAudioSource(soundRequest.audioSourceAssign);
        soundRequest.audioSourceAssign = null;
    }

    /// <summary>
    /// Joue le son définit
    /// </summary>
    /// <param name="soundRequest"></param>
    /// <param name="positionSound"></param>
    public void PlaySound(SoundRequest soundRequest, Vector3 positionSound)
    {
        var audioSource = PickAudioSource();

        if (audioSource == null ||soundRequest == null) return;

        switch (soundRequest.category)
        {
            case SoundCategory.MUSIC:
                audioSource.outputAudioMixerGroup = mixerGroups[0];
                break;
            case SoundCategory.SFX:
                audioSource.outputAudioMixerGroup = mixerGroups[1];
                break;
            case SoundCategory.AMBIENT:
                audioSource.outputAudioMixerGroup = mixerGroups[2];
                break;
        }

        audioSource.clip = soundRequest.audio;
        audioSource.priority = soundRequest.priority;
        audioSource.volume = soundRequest.volume;
        audioSource.spatialBlend = soundRequest.spatializeSound;
        audioSource.loop = soundRequest.looping;
        audioSource.transform.position = positionSound;

        audioSource.Play();

        if (soundRequest.looping) return;

        StartCoroutine(Utils.Delay(()=> FreeAudioSource(audioSource), soundRequest.audio.length));
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <returns></returns>
    private AudioSource PullAudioSource()
    {
        var go = new GameObject("AudioSource");
        go.transform.parent = transform;

        return go.AddComponent<AudioSource>();
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <returns></returns>
    private AudioSource PickAudioSource()
    {
        AudioSource audioSource = null;

        if (canExpendQueue) audioSource = PullAudioSource();
        else if (audioSourcesAvailable.Count > 0) audioSource =  audioSourcesAvailable.Dequeue();
        else Debug.LogWarning(message: "None AudioSource Available");

        return audioSource;
    }

    /// <summary>
    /// ??? (A compléter)
    /// </summary>
    /// <param name="audioSource"></param>
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
