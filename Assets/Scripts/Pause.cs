using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private static bool _paused = false;
    public static bool Paused => _paused;
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] private float _timeToEnableColliders = .2f;
    [SerializeField] private Material _material;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioUI _audioUI;

    private void Start()
    {
        _enemies = new List<Enemy>(GetComponentsInChildren<Enemy>());
        _audioSource = GetComponent<AudioSource>();
        EnableEnemies(false);
    }

    private void Update()
    {
        HandlePause();
    }

    private void HandlePause()
    {
        if (!GameManager.Instance.OnGame || PowerUpManager.Instance.OnPowerUpMenu) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool enemiesEnabled = false;
            foreach (Enemy enemy in _enemies)
                enemiesEnabled |= enemy.gameObject.activeSelf;
            
            if (enemiesEnabled) return;
            if (!_paused)
            {
                PauseGame();
                return;
            }

            if (_paused) 
                ResumeGame();
        }

        if (_paused)
        {
            if (Input.GetMouseButtonDown(0))
                ResumeGame();
        }
    }

    private void PauseGame()
    {
        _audioUI.Enable();
        AudioHelper.Instance.SmoothAudio(_audioSource, .3f, .4f, false, true);
        _paused = true;
        AudioHelper.Instance.SmoothLowPass(0.1f, .5f);

        if (!Player.Instance.IsInvincible)
            GameManager.Instance.AnimateMaterial("_OldTV", 0, 0.005f, .2f);

        ResetEnemies();
        EnableColliders(false);
        EnableEnemies(true);
    }

    private void ResumeGame()
    {
        _audioUI.Disable();
        AudioHelper.Instance.SmoothAudio(_audioSource, 0f, .4f, true, true);
        AudioHelper.Instance.SmoothLowPass(1f, .5f);

        if (!Player.Instance.IsInvincible)
            GameManager.Instance.AnimateMaterial("_OldTV", 0.005f, 0, .2f);
        
        _paused = false;
        StartCoroutine(WaitToEnableColliders(_timeToEnableColliders));
    }

    public void OnGameStart()
    {
        _paused = false;
        EnableEnemies(false);
        ResetEnemies();
    }

    private void ResetEnemies()
    {
        float x = -1.5f;
        foreach (Enemy enemy in _enemies)
        {
            x += 1.0f;
            enemy.Reset(new Vector3(x, 0, 0));
        }
    }

    private void EnableEnemies(bool enable)
    {
        foreach (Enemy enemy in _enemies)
        {
            enemy.gameObject.SetActive(enable);
            if (enable) 
                enemy.SmoothAppear(.1f);
        }
    }

    private IEnumerator WaitToEnableColliders(float time)
    {
        yield return new WaitForSeconds(time);
        EnableColliders(true);
    }

    private void EnableColliders(bool enable)
    {
        foreach (Enemy enemy in _enemies)
            enemy.EnableCollider(enable);
    }
}
