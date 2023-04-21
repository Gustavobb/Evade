using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private static bool _paused = false;
    public static bool Paused => _paused;
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();
    [SerializeField] private float _timeToEnableColliders = .2f;

    private void Start()
    {
        _enemies = new List<Enemy>(GetComponentsInChildren<Enemy>());
        EnableEnemies(false);
    }

    private void Update()
    {
        HandlePause();
    }

    private void HandlePause()
    {
        if (!GameManager.Instance.OnGame) return;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            bool enemiesEnabled = false;
            foreach (Enemy enemy in _enemies)
                enemiesEnabled |= enemy.gameObject.activeSelf;
            
            if (enemiesEnabled) return;
            _paused = !_paused;

            if (_paused)
            {
                ResetEnemies();
                EnableColliders(false);
                EnableEnemies(true);
            }

            if (!_paused) StartCoroutine(WaitToEnableColliders(_timeToEnableColliders));
        }

        if (_paused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _paused = false;
                StartCoroutine(WaitToEnableColliders(_timeToEnableColliders));
            }
        }
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
