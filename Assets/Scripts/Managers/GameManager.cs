using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    [SerializeField] private UnityEvent _onGameStart;
    [SerializeField] private GameObject menuUI;
    
    [SerializeField] private Material _shaderMaterial;
    private bool onGame;
    public bool OnGame => onGame;
    private AudioSource _audioSource;
    public AudioSource _AudioSource => _audioSource;
    [SerializeField] private AudioSource _startAudioSource;
    
    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        ActivateMenu();
    }

    private void Update()
    {
        HandleReset();
        HandleMenu();
    }

    private void HandleReset()
    {
        if (Player.Instance == null) return;
        
        if (!onGame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Player.Instance.Reset();
                EnemyManager.Instance.Reset(false);
            }
        }
    }

    public void ShakeScreen(float time)
    {
        StartCoroutine(ShakeScreenCoroutine(time));
    }

    private IEnumerator ShakeScreenCoroutine(float time)
    {
        _shaderMaterial.SetFloat("_ScreenShake", 0.1f);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0.007f);
        yield return new WaitForSeconds(time);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
        _shaderMaterial.SetFloat("_ScreenShake", 0f);
    }

    private void HandleMenu()
    {
        if (!onGame & Input.GetMouseButtonDown(0)){
            DestroyMenu();
        }
    }

    private void DestroyMenu()
    {
        _onGameStart.Invoke();
        menuUI.SetActive(false);
        onGame = true;
    }

    protected void ActivateMenu()
    {
        menuUI.SetActive(true);
        _startAudioSource.Play();
        onGame = false;
        StartCoroutine(AnimateLensDistortion());
    }

    public void GameOver()
    {
        ActivateMenu();
        // matar inimigos
        // resetar power ups
    }

    private IEnumerator AnimateLensDistortion()
    {
        float time = 0;
        float duration = .19f;
        float startValue = 1f;
        float endValue = 1.03f;
        float op = 1f;

        while (Application.isPlaying)
        {
            if (time >= duration) op = -1f;
            if (time <= 0) op = 1f;

            time += op * Time.deltaTime;
            float value = Mathf.Lerp(startValue, endValue, time / duration);
            _shaderMaterial.SetFloat("_LensDistortion", value);
            yield return null;
        }

        _shaderMaterial.SetFloat("_LensDistortion", 1f);
    }

    public void RequestWobble()
    {
        if (!PowerUpManager.Instance.OnPowerUpMenu)
        {
            GameBounds.Instance.PlaySound();
            _shaderMaterial.SetFloat("_Wobble", 0.007f);
            _shaderMaterial.SetFloat("_WobbleFrequency", 0.4f);
            _shaderMaterial.SetFloat("_WobbleAmplitude", 1.18f);
            _shaderMaterial.SetFloat("_OldTV", 0.007f);
        }
    }

    public void AnimateGreyScale(float start, float end, float duration)
    {
        StartCoroutine(AnimateGreyScaleCoroutine(start, end, duration));
    }

    public IEnumerator AnimateGreyScaleCoroutine(float start, float end, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Lerp(start, end, time / duration);
            _shaderMaterial.SetFloat("_GreyScale", value);
            yield return null;
        }

        _shaderMaterial.SetFloat("_GreyScale", end);
    }

    public void SlowDown()
    {
        _shaderMaterial.SetFloat("_OldTV", 0.007f);
    }

    public void StopSlowDown()
    {
        _shaderMaterial.SetFloat("_OldTV", 0f);
    }

    public void RequestPowerUpPP()
    {
        _shaderMaterial.SetFloat("_Wobble", 0.007f);
        _shaderMaterial.SetFloat("_WobbleFrequency", 4.78f);
        _shaderMaterial.SetFloat("_WobbleAmplitude", 0.89f);
    }

    public void StopPowerUpPP()
    {
        _shaderMaterial.SetFloat("_Wobble", 0f);
    }

    public void StopWobble()
    {
        _shaderMaterial.SetFloat("_Wobble", 0f);
        _shaderMaterial.SetFloat("_OldTV", 0f);
    }
}
