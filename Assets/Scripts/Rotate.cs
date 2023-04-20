using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    private bool _originalRotate;
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private bool _circularRotation = false;
    [SerializeField] private bool _sinusoidalRotation = false;
    [SerializeField] private float _sinusoidalAmplitude = 5f;
    [SerializeField] private float _sinusoidalOffset = 5f;
    [SerializeField] private bool _playInPause = false;
    private Quaternion _originalRotation;

    private void Awake()
    {
        Setup();
        _originalRotation = _rotationPivot.rotation;
    }

    private void OnEnable()
    {
        _rotationPivot.rotation = _originalRotation;
    }
    
    private void Setup()
    {
        if (_rotationPivot == null)
            _rotationPivot = transform;
    }

    private void Update()
    {
        if (!_playInPause && !GameManager.Instance.OnGame) return;
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
