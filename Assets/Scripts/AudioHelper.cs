using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
