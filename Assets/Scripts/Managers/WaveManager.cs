using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class WaveManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _waveTimerText, _waveTimerTextShadow, _waveCountText, _waveCountTextShadow;
    [SerializeField] private float _waveTimeStart = 10f;
    [SerializeField] private float _waveTimeConstant = 2f;
    [SerializeField] private float _timeScale = 1f;
    [SerializeField] private UnityEvent _onWaveChange, _onWaveStart;
    [SerializeField] private List<Wave> _waves;
    private Wave _currentWave;

    [SerializeField] private int _waveNumber = 0, _waveNumberTotal = 1;
    [SerializeField] private EnemyData _enemyData;
    [SerializeField] private GuardianData _guardianData;
    public EnemyData EnemyData => _enemyData;
    public float _waveTime;
    [SerializeField] private GameObject _mouseUI;

    private static bool _pastWave2 = false;

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
        _currentWave.Setup(true);
        _waveCountText.text = $"WAVE {_waveNumberTotal}";
        _waveCountTextShadow.text = $"WAVE {_waveNumberTotal}";
    }

    private void Update()
    {
        if (Player.Instance == null || !GameManager.Instance.OnGame || Pause.Paused || PowerUpManager.Instance.OnPowerUpMenu || Player.Instance.IsInvincible) return;
        HandleWaveTime();
        _currentWave.WaveLogic();
    }

    private void HandleWaveTime()
    {
        _waveTimerText.text = _waveTime.ToString("F1");
        _waveTimerTextShadow.text = _waveTime.ToString("F1");
        _waveTime -= Time.deltaTime * _timeScale;

        if (_waveTime <= 0)
            _onWaveChange?.Invoke();
    }

    public void SetupWave()
    {
        _currentWave.VanishWave();

        _waveNumber ++;
        _waveNumberTotal ++;
        _waveCountText.text = $"WAVE {_waveNumberTotal}";
        _waveCountTextShadow.text = $"WAVE {_waveNumberTotal}";

        if (_waveNumber == 1 && !_pastWave2)
            StartCoroutine(DelayTutorial());

        if (_waveNumber >= _waves.Count) _waveNumber = _waves.Count - 1;
        _waveTime = _waveTimeStart + _waveTimeConstant * _waveNumber;
        _currentWave = _waves[_waveNumber];
        _currentWave.Setup(true);
        _onWaveStart?.Invoke();
    }

    public void DisableTutorial()
    {
        _mouseUI.SetActive(false);
        _pastWave2 = true;
    }

    private IEnumerator DelayTutorial()
    {
        yield return new WaitForSeconds(3f);
        if (!GameManager.Instance.OnGame) yield break;
        _mouseUI.SetActive(true);
    }

    public void ResetWaves()
    {
        _waveNumber = 0;
        _waveNumberTotal = 1;
        _waveCountText.text = $"WAVE {_waveNumberTotal}";
        _waveCountTextShadow.text = $"WAVE {_waveNumberTotal}";
        _currentWave = _waves[_waveNumber];
        _currentWave.Setup(false);
        _waveTime = _waveTimeStart;
    }

    public void SetTimeScale(float timeScale)
    {
        _timeScale = timeScale;
    }

    private void OnApplicationQuit()
    {
        _enemyData.Reset();
        _guardianData.Reset();
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
    public bool randomPallette = false;
    public int palletteIndex = 0;
    public List<Enemy.EnemyType> possibleEnemyTypes;
    
    public void Setup(bool rand)
    {
        spawnRateCountdown = 0f;
        EnemyManager.Instance.ChangeMaxActiveEnemies(maxEnemyCount);
        WaveManager.Instance.EnemyData.MultiplySpeed(enemySpeedMultiplier);
        WaveManager.Instance.EnemyData.MultiplySize(enemySizeMultiplier);
        if (usePallette){
            if (!rand) return;
            if(randomPallette){
                int idx = Random.Range(0, PaletteManager.Instance.palettes.Count);
                PaletteManager.Instance.ApplyPalette(idx);
                return;
            }
            PaletteManager.Instance.ApplyPalette(palletteIndex);
        }
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