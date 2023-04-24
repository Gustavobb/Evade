using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockManager : MonoBehaviour
{
    [SerializeField] public int MAX_CLOCKS = 3;
    [SerializeField] public int _clockCount = 1;
    [SerializeField] private float _timeScale = 0.5f;
    [SerializeField] private float _duration = 5f;
    [SerializeField] private EnemyData _enemyData;
    private int _clocksActive = 0;
    private AudioSource _audioSource;

    private static ClockManager _instance;
    public static ClockManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<ClockManager>();
            return _instance;
        }
    }

    private List<GameObject> _clocks = new List<GameObject>();

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        _clocksActive = _clockCount;
        for (int i = 0; i < transform.childCount; i++)
        {
            _clocks.Add(transform.GetChild(i).gameObject);
            if (i >= _clockCount) _clocks[i].SetActive(false);
        }
    }

    private void Update()
    {
        HandleSlowDown();
    }

    private void HandleSlowDown()
    {
        if (!GameManager.Instance.OnGame || PowerUpManager.Instance.OnPowerUpMenu || Pause.Paused) return;
        if (Input.GetMouseButtonDown(0))
            UseClock();
    }

    public void UseClock()
    {
        if (Player.Instance.IsInvincible) return;
        if (_clocksActive == 0) return;

        _clocksActive --;
        _clocks[_clocksActive].SetActive(false);
        _audioSource.Play();
        
        StartCoroutine(SlowDownCoroutine());
    }

    public void WaveReset()
    {
        _clocksActive = _clockCount;
        for (int i = 0; i < _clocksActive; i++)
            _clocks[i].SetActive(true);
    }

    private IEnumerator SlowDownCoroutine()
    {
        _enemyData.MultiplySpeed(_timeScale);
        WaveManager.Instance.SetTimeScale(_timeScale);
        GameManager.Instance.AnimateMaterial("_GreyScale", 0f, 1f, .3f);
        GameManager.Instance.AnimateMaterial("_ColorDecay", 1f, .2f, .3f);
        AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, _timeScale, .4f, false, false);
        
        float timer = 0f;
        while (timer < _duration)
        {
            GameManager.Instance.SlowDown();
            if (PowerUpManager.Instance.OnPowerUpMenu || !GameManager.Instance.OnGame) break;
            if (Pause.Paused) timer -= Time.deltaTime;
            timer += Time.deltaTime;

            yield return null;
        }
        
        WaveManager.Instance.SetTimeScale(1f);
        _enemyData.MultiplySpeed(1f);
        GameManager.Instance.StopSlowDown();
        GameManager.Instance.AnimateMaterial("_GreyScale", 1f, 0f, .3f);
        GameManager.Instance.AnimateMaterial("_ColorDecay", .2f, 1f, .3f);
        AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, 1, .4f, false, false);
    }
}
