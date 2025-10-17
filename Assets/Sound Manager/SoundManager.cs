using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    Dictionary<string, AudioClip> sfxAudioClipsDic = new();
    Dictionary<string, AudioClip> bgmAudioClipsDic = new();
    Dictionary<string, GameObject> usedAudio = new();

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

        AudioClip audioClip = GetSFXAudioClip(srcName);
        if(audioClip == null)
        {
            Debug.LogError(srcName + " is not found");
            yield break;
        }

        float coolDown;
        GameObject obj = null;

        if (usedAudio.ContainsKey(name))
        {
            var audio = usedAudio[name];
            audio.SetActive(true);

            float elapsedTime = 0f;
            coolDown = audioClip.length;
            while (elapsedTime <= coolDown)
            {
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            audio.SetActive(false);
        }
        else
        {
            obj = new GameObject(name);
            obj.AddComponent<AudioSource>();

            AudioSource audioSource = obj.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = isLoop;
            audioSource.spatialBlend = 1f;

            usedAudio.Add(name, obj);
            audioSource.Play();

            if (obj != null)
            {
                float elapsedTime = 0f;
                coolDown = audioClip.length;
                while (elapsedTime <= coolDown)
                {
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                audioSource.Stop();
                obj.SetActive(false);
            }
        }
    }

    public void PlayBGMSound(string srcName, bool isLoop = true)
    {
        string name = "BGM Sound Player" + srcName;

        AudioClip audioClip = GetBGMAudioClip(srcName);
        if (audioClip == null)
        {
            Debug.LogError(srcName + " is not found");
            return;
        }

        GameObject obj = null;

        if (usedAudio.ContainsKey(name))
        {
            var audio = usedAudio[name];
            audio.SetActive(true);

            audio.GetComponent<AudioSource>().Stop();
            audio.GetComponent<AudioSource>().Play();
        }
        else
        {
            obj = new GameObject(name);
            obj.AddComponent<AudioSource>();

            AudioSource audioSource = obj.GetComponent<AudioSource>();
            audioSource.clip = audioClip;
            audioSource.loop = isLoop;
            audioSource.volume = 0.5f;

            usedAudio.Add(name, obj);
            audioSource.Play();
        }
    }

    public void StopBGMSound()
    {
        usedAudio["BGM Sound Player" + "sfx_swooshing"].GetComponent<AudioSource>().Stop();
    }
}
