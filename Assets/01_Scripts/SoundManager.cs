using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    Dictionary<string, AudioClip> sfxAudioClipsDic = new();
    Dictionary<string, AudioClip> bgmAudioClipsDic = new();

    public List<GameObject> instantiatedAudioSources = new();

    [Header("Audio Clips")]
    [SerializeField] AudioClip[] preloadSFXs;
    [SerializeField] AudioClip[] preloadBGMs;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);

        foreach(AudioClip clip in preloadSFXs)
        {
            sfxAudioClipsDic.Add(clip.name, clip);
        }

        foreach(AudioClip clip in preloadBGMs)
        {
            bgmAudioClipsDic.Add(clip.name, clip);
        }
    }

    AudioClip GetSFXAudioClip(string srcName)
    {
        AudioClip src = sfxAudioClipsDic[srcName];
        if(src == null) Debug.LogError("Audio Clip not found : " + srcName);

        return src;
    }

    AudioClip GetBGMAudioClip(string srcName)
    {
        AudioClip src = bgmAudioClipsDic[srcName];
        if(src == null) Debug.LogError("Audio Clip not found : " + srcName);

        return src;
    }

    public void PlaySFXSound(string srcName, bool isLoop = false)
    {
        StartCoroutine(Co_PlaySFXSound(srcName, isLoop));
    }

    private IEnumerator Co_PlaySFXSound(string srcName, bool isLoop)
    {
        string name = "SFX Sound Player" + srcName;
        GameObject obj = new (name);

        if (instantiatedAudioSources.Contains(obj))
        {
            obj = instantiatedAudioSources[instantiatedAudioSources.IndexOf(obj)];
            obj.SetActive(true);
        }
        else
        {
            obj.AddComponent<AudioSource>();

            AudioSource audioSource = obj.GetComponent<AudioSource>();
            audioSource.clip = GetSFXAudioClip(srcName);
            audioSource.loop = isLoop;
            audioSource.spatialBlend = 1f;

            instantiatedAudioSources.Add(obj);
            audioSource.Play();

            float elapsedTime = 0f;
            while (elapsedTime <= audioSource.clip.length - .1f)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            obj.SetActive(false);
        }  
    }

    public void PlayBGMSound(string srcName, bool isLoop = true)
    {
        GameObject obj = new ("BGM Sound Player" + srcName);
        obj.AddComponent<AudioSource>();

        AudioSource audioSource = obj.GetComponent<AudioSource>();
        audioSource.clip = GetBGMAudioClip(srcName);
        audioSource.loop = isLoop;

        audioSource.Play();
    }
}
