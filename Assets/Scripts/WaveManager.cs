using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveTimerText;
    [SerializeField] private float _waveTimeStart = 10f;
    [SerializeField] private float _waveTimeConstant = 2f;
    [SerializeField] private UnityEvent _onWaveChange;
    [SerializeField] private List<Wave> _waves;
    
    private float _waveTime;
    private int _waveNumber = 0;

    private static WaveManager _instance;
    public static WaveManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<WaveManager>();
            return _instance;
        }
    }

    private void Start()
    {
        _waveTime = _waveTimeStart;
    }

    private void Update()
    {
        if (Player.Instance == null || !GameManager.Instance.OnGame || Pause.Paused) return;
        HandleWaveTime();
    }

    private void HandleWaveTime()
    {
        _waveTimerText.text = _waveTime.ToString("F1");
        _waveTime -= Time.deltaTime;

        if (_waveTime <= 0)
        {
            _waveNumber ++;
            _waveTime = _waveTimeStart + _waveTimeConstant * _waveNumber;
            _onWaveChange?.Invoke();
        }
    }

    public void ResetWaves()
    {
        _waveNumber = 0;
        _waveTime = _waveTimeStart;
    }
}

[System.Serializable]
public class Wave
{
    public int enemyCount;
    public float spawnRate;
    public List<Enemy> possibleEnemies;
}