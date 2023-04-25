using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioUI : MonoBehaviour
{
    [SerializeField] private float _volume = 1f;
    [SerializeField] private GameObject _volumeSlider;
    private Vector3 sliderScale;

    private void Start()
    {
        sliderScale = _volumeSlider.transform.localScale;
    }

    private void OnMouseOver()
    {
        if (Input.mouseScrollDelta.y < 0f)
        {
            _volume += Time.deltaTime;
            _volume = Mathf.Clamp(_volume, 0.00001f, 1f);
            _volumeSlider.transform.localScale = new Vector3(_volume * sliderScale.x, sliderScale.y, sliderScale.z);
        }
        else if (Input.mouseScrollDelta.y > 0f)
        {
            _volume -= Time.deltaTime;
            _volume = Mathf.Clamp(_volume, 0.00001f, 1f);
            _volumeSlider.transform.localScale = new Vector3(_volume * sliderScale.x, sliderScale.y, sliderScale.z);
        }

        AudioHelper.Instance.SetMasterVolume(_volume);
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
