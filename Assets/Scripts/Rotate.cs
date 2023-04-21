using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private bool _originalRotate;
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private float _lerpTime = .5f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _circularRotation = false;
    [SerializeField] private bool _sinusoidalRotation = false;
    [SerializeField] private float _sinusoidalAmplitude = 5f;
    [SerializeField] private float _sinusoidalOffset = 5f;
    [SerializeField] private bool _playInPause = false;
    [SerializeField] private bool _alwaysResetRotation = true;
    private Quaternion _originalRotation;

    private void Awake()
    {
        Setup();
        _originalRotation = _rotationPivot.rotation;
    }

    private void OnEnable()
    {
        if (_alwaysResetRotation) _rotationPivot.rotation = _originalRotation;
    }

    public void LerpRotationSpeed()
    {
        StartCoroutine(LerpRotationSpeed(0, _rotationSpeed));
    }

    private IEnumerator LerpRotationSpeed(float start, float end)
    {
        float time = 0;
        while (time < _lerpTime)
        {
            _rotationSpeed = Mathf.Lerp(start, end, time / _lerpTime);
            time += Time.deltaTime;
            yield return null;
        }
        _rotationSpeed = end;
    }
    
    private void Setup()
    {
        if (_rotationPivot == null)
            _rotationPivot = transform;
    }

    private void Update()
    {
        if (PowerUpManager.Instance.OnPowerUpMenu) return;
        if ((Pause.Paused || !GameManager.Instance.OnGame) && !_playInPause) return;
        if (_circularRotation) CircularRotation();
        if (_sinusoidalRotation) SinusoidalRotation();
    }

    private void CircularRotation()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, _rotationSpeed * Time.deltaTime);
        _rotationPivot.localRotation *= rotation;
    }

    private void SinusoidalRotation()
    {
        float rotation = Mathf.Sin(Time.time * _rotationSpeed + _sinusoidalOffset) * _sinusoidalAmplitude;
        _rotationPivot.rotation *= Quaternion.Euler(0, 0, rotation);
    }
}
