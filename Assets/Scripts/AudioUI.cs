using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUI : MonoBehaviour
{
    [SerializeField] private float _volume = .5f;
    [SerializeField] private GameObject _volumeSlider;
    private Vector3 sliderScale;

    private void Start()
    {
        sliderScale = _volumeSlider.transform.localScale;
        SetAudio(_volume);
    }

    private void OnMouseOver()
    {
        if (Input.mouseScrollDelta.y < 0f)
            _volume += Time.deltaTime;
        else if (Input.mouseScrollDelta.y > 0f)
            _volume -= Time.deltaTime;

        SetAudio(_volume);
    }

    private void SetAudio(float vol)
    {
        vol = Mathf.Clamp(vol, 0.00001f, 1f);
        _volumeSlider.transform.localScale = new Vector3(vol * sliderScale.x, sliderScale.y, sliderScale.z);
        AudioHelper.Instance.SetMasterVolume(vol);
    }

    public void Disable()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public void Enable()
    {
        foreach (Transform child in transform)
            child.gameObject.SetActive(true);
    }
}
