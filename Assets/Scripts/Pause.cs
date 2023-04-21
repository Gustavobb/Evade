using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private static bool _paused = false;
    public static bool Paused => _paused;
    [SerializeField] private List<Enemy> _enemies = new List<Enemy>();

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

            if (!_paused)
            {
                ResetEnemies();
                EnableEnemies(true);
            }

            _paused = !_paused;
        }

        if (_paused)
        {
            if (Input.GetMouseButtonDown(0))
                _paused = false;
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
            if (enable) enemy.SmoothAppear(.1f);
        }
    }
}
