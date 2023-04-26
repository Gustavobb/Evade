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
    [SerializeField] private GameObject clickUI, titleUI;
    [SerializeField] private AudioUI audioUI;
    
    [SerializeField] private Material _shaderMaterial;
    private bool onGame;
    public bool OnGame => onGame;
    private AudioSource _audioSource;
    public AudioSource _AudioSource => _audioSource;
    [SerializeField] private AudioSource _startAudioSource;
    public bool wobbling = false;
    
    private void Start() {
        _audioSource = GetComponent<AudioSource>();
        ActivateMenu();
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
        _shaderMaterial.SetFloat("_ScreenShake", 0f);
        _shaderMaterial.SetFloat("_ColorDecay", 1f);
        _shaderMaterial.SetFloat("_Wobble", 0f);
        _shaderMaterial.SetFloat("_OldTV", 0f);
        _shaderMaterial.SetFloat("_GreyScale", 0);
        _shaderMaterial.SetFloat("_HueShift", 0);
    }

    private void LateUpdate()
    {
        HandleReset();
        HandleMenu();
        HandleClosestEnemy();
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

    public void HandleClosestEnemy()
    {
        bool haveSlowDown=false;
        int slowDownPowerUpIndex=0;
        for (int i=0; i < PowerUpManager.Instance.allPowerUps.Count; i++)
        {
            if (PowerUpManager.Instance.allPowerUps[i].GetName() == "SlowDownPowerUp"){
                haveSlowDown = true;
                slowDownPowerUpIndex = i;
            }
        }
        if (Player.Instance.GetClosestEnemy()!=null){
            if ((Player.Instance.GetClosestEnemy().GetToPlayer().magnitude <= 1.5f)&(haveSlowDown)){
                PowerUpManager.Instance.allPowerUps[slowDownPowerUpIndex].ActivatePowerUp();
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
        audioUI.Disable();
        titleUI.SetActive(false);
        clickUI.SetActive(false);
        onGame = true;
    }

    protected void ActivateMenu()
    {
        titleUI.SetActive(true);
        clickUI.SetActive(true);
        audioUI.Enable();
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
            wobbling = true;
            AnimateMaterial("_Wobble", 0f, 0.007f, 0.2f);
            _shaderMaterial.SetFloat("_WobbleFrequency", 0.4f);
            _shaderMaterial.SetFloat("_WobbleAmplitude", 0.7f);
            AnimateMaterial("_OldTV", 0f, 0.004f, 0.2f);
        }
    }

    public void AnimateMaterial(string name, float start, float end, float duration)
    {
        StartCoroutine(AnimateMaterialCoroutine(name, start, end, duration));
    }

    public IEnumerator AnimateMaterialCoroutine(string name, float start, float end, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            time += Time.deltaTime;
            float value = Mathf.Lerp(start, end, time / duration);
            _shaderMaterial.SetFloat(name, value);
            yield return null;
        }

        _shaderMaterial.SetFloat(name, end);
    }

    public void SlowDown()
    {
        _shaderMaterial.SetFloat("_ColorDecay", .2f);
        _shaderMaterial.SetFloat("_OldTV", 0.007f);
    }

    public void StopSlowDown()
    {
        _shaderMaterial.SetFloat("_ColorDecay", 1f);
        _shaderMaterial.SetFloat("_OldTV", 0f);
    }

    public void RequestPowerUpPP()
    {
        AnimateMaterial("_Wobble", 0f, 0.007f, 0.2f);
        _shaderMaterial.SetFloat("_WobbleFrequency", 3.92f);
        _shaderMaterial.SetFloat("_WobbleAmplitude", 0.23f);
    }

    public void StopPowerUpPP()
    {
        AnimateMaterial("_Wobble", 0.007f, 0f, 0.2f);
    }

    public void StopWobble()
    {
        wobbling = false;
        if (!Player.Instance.IsInvincible && ! Pause.Paused) AnimateMaterial("_OldTV", 0.004f, 0f, 0.2f);
        if (PowerUpManager.Instance.OnPowerUpMenu) return;
        AnimateMaterial("_Wobble", 0.007f, 0f, 0.2f);
    }
}
