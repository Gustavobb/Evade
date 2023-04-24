using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioHelper : MonoBehaviour
{
    private static AudioHelper _instance;
    public static AudioHelper Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<AudioHelper>();
            return _instance;
        }
    }
    
    [SerializeField] private AudioMixer _audioMixer;

    public void SmoothAudio(AudioSource audioSource, float targetVar, float duration, bool stop, bool volume)
    {
        StartCoroutine(SmoothVolume(audioSource, targetVar, duration, stop, volume));
    }

    private IEnumerator SmoothVolume(AudioSource audioSource, float targetVar, float duration, bool stop, bool volume)
    {
        if (!stop && !audioSource.isPlaying)
            audioSource.Play();

        float initialVar = volume ? audioSource.volume : audioSource.pitch; 
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float val = Mathf.Lerp(initialVar, targetVar, time / duration);
            if (volume) audioSource.volume = val;
            else audioSource.pitch = val;
            yield return null;
        }

        if (volume) audioSource.volume = targetVar;
        else audioSource.pitch = targetVar;

        if (stop && audioSource.isPlaying)
            audioSource.Stop();
    }

    public void SmoothLowPass(float targetVar, float duration)
    {
        StartCoroutine(SmoothLowPassFilter(targetVar, duration));
    }

    public IEnumerator SmoothLowPassFilter(float targetVar, float duration)
    {
        float initialVar = _audioMixer.GetFloat("MasterFreq", out float value) ? value : 0;
        float max = 22000;
        initialVar /= max;

        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float val = Mathf.Lerp(initialVar, targetVar, time / duration);
            SetMasterFreq(val);
            yield return null;
        }

        SetMasterFreq(targetVar);
    }

    public void SetMasterVolume(float volume)
    {
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20);
    }

    public void SetMasterFreq(float freq)
    {
        float max = 22000;
        freq *= max;
        _audioMixer.SetFloat("MasterFreq", freq);
    }
}
