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
    private Wave _currentWave;

    private float _waveTime;
    [SerializeField] private int _waveNumber = 0;
    [SerializeField] private EnemyData _enemyData;
    public EnemyData EnemyData => _enemyData;

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
        _currentWave = _waves[_waveNumber];
        _currentWave.Setup();
    }

    private void Update()
    {
        if (Player.Instance == null || !GameManager.Instance.OnGame || Pause.Paused || PowerUpManager.Instance.OnPowerUpMenu) return;
        HandleWaveTime();
        _currentWave.WaveLogic();
    }

    private void HandleWaveTime()
    {
        _waveTimerText.text = _waveTime.ToString("F1");
        _waveTime -= Time.deltaTime;

        if (_waveTime <= 0)
            _onWaveChange?.Invoke();
    }

    public void SetupWave()
    {
        _currentWave.VanishWave();

        _waveNumber ++;
        if (_waveNumber >= _waves.Count) _waveNumber = _waves.Count - 1;
        _waveTime = _waveTimeStart + _waveTimeConstant * _waveNumber;
        _currentWave = _waves[_waveNumber];
        _currentWave.Setup();
    }

    public void ResetWaves()
    {
        _waveNumber = 0;
        _currentWave = _waves[_waveNumber];
        _currentWave.Setup();
        _waveTime = _waveTimeStart;
    }

    private void OnApplicationQuit()
    {
        _enemyData.Reset();
    }
}

[System.Serializable]
public class Wave
{
    public int maxEnemyCount;
    public float spawnRate;
    public float enemySpeedMultiplier = 1f;
    public float enemySizeMultiplier = 1f;
    private float spawnRateCountdown = 0f;
    public bool usePallette = false;
    public int palletteIndex = 0;
    public List<Enemy.EnemyType> possibleEnemyTypes;
    
    public void Setup()
    {
        EnemyManager.Instance.ChangeMaxActiveEnemies(maxEnemyCount);
        WaveManager.Instance.EnemyData.MultiplySpeed(enemySpeedMultiplier);
        WaveManager.Instance.EnemyData.MultiplySize(enemySizeMultiplier);
        if (usePallette) PaletteManager.Instance.ApplyPalette(palletteIndex);
    }

    public void WaveLogic()
    {
        spawnRateCountdown -= Time.deltaTime;

        if (spawnRateCountdown <= 0)
        {
            spawnRateCountdown = spawnRate;
            SpawnEnemy();
        }
    }

    public void SpawnEnemy()
    {
        int randomIndex = Random.Range(0, possibleEnemyTypes.Count);
        EnemyManager.Instance.SpawnEnemy(possibleEnemyTypes[randomIndex]);
    }

    public void VanishWave()
    {
        EnemyManager.Instance.Reset(false);
    }
}