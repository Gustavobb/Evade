using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int MAX_ACTIVE_ENEMIES = 10;
    
    private List<Enemy> _enemiesPool = new List<Enemy>();
    private List<Enemy> _inactiveEnemies = new List<Enemy>();

    [SerializeField] private List<Vector4> _spawnRects = new List<Vector4>();

    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<EnemyManager>();
            return _instance;
        }
    }

    private void Start()
    {
        Reset();
    }

    public void AddEnemy(Enemy enemy)
    {
        _enemiesPool.Add(enemy);

        if (_enemiesPool.Count > MAX_ACTIVE_ENEMIES)
            enemy.gameObject.SetActive(false);
    }

    public void RemoveEnemy(Enemy enemy)
    {
        _enemiesPool.Remove(enemy);

        if (_inactiveEnemies.Contains(enemy))
            _inactiveEnemies.Remove(enemy);
    }

    public Enemy RequestAvailableEnemy()
    {
        if (_inactiveEnemies.Count == 0) return null;
        
        Enemy enemy = _inactiveEnemies[0];
        enemy.gameObject.SetActive(true);
        return enemy;
    }

    public void SpawnEnemy()
    {
        int activeEnemies = _enemiesPool.Count - _inactiveEnemies.Count;
        if (activeEnemies >= MAX_ACTIVE_ENEMIES) return;

        Enemy enemy = RequestAvailableEnemy();
        if (enemy == null) return;
        
        Vector4 spawnRect = _spawnRects[Random.Range(0, _spawnRects.Count)];
        Vector3 spawnPos = new Vector3(Random.Range(spawnRect.x, spawnRect.z), Random.Range(spawnRect.y, spawnRect.w), 0f);
        enemy.Reset(spawnPos);
    }

    public void KillEnemy(Enemy enemy)
    {
        enemy.gameObject.SetActive(false);
        // ?
        SpawnEnemy();
    }

    public void Reset()
    {
        foreach (Enemy enemy in _enemiesPool)
        {
            enemy.Reset(Vector3.zero);
            enemy.gameObject.SetActive(false);
        }
        
        for (int i = 0; i < MAX_ACTIVE_ENEMIES; i++)
            SpawnEnemy();
    }
    
    public void AddInactiveEnemie(Enemy enemy)
    {
        _inactiveEnemies.Add(enemy);
    }

    public void RemoveInactiveEnemie(Enemy enemy)
    {
        _inactiveEnemies.Remove(enemy);
    }
}
