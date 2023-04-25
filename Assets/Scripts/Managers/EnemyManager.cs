using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int MAX_ACTIVE_ENEMIES = 10;
    [SerializeField] private List<EnemyPool> _enemyPools = new List<EnemyPool>();
    [SerializeField] private List<Vector4> _spawnRects = new List<Vector4>();
    [SerializeField] private EnemyData _enemyData;
    public EnemyData enemyData => _enemyData;

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
        Setup();
        Reset(false);
    }

    private void Setup()
    {
        foreach (EnemyPool enemyPool in _enemyPools)
            enemyPool.Setup();
    }

    public Enemy RequestAvailableEnemy(Enemy.EnemyType type)
    {
        foreach (EnemyPool enemyPool in _enemyPools)
        {
            if (enemyPool._type == type)
            {
                if (enemyPool._inactiveEnemies.Count == 0) return null;
                Enemy enemy = enemyPool._inactiveEnemies[0];
                enemy.gameObject.SetActive(true);
                return enemy;
            }
        }

        return null;
    }

    public void SpawnEnemy(Enemy.EnemyType type)
    {
        int enemiesSum = 0;
        int inactiveEnemiesSum = 0;

        foreach (EnemyPool enemyPool in _enemyPools)
        {
            enemiesSum += enemyPool._enemies.Count;
            inactiveEnemiesSum += enemyPool._inactiveEnemies.Count;
        }

        int activeEnemies = enemiesSum - inactiveEnemiesSum;
        if (activeEnemies >= MAX_ACTIVE_ENEMIES) return;

        Enemy enemy = RequestAvailableEnemy(type);
        if (enemy == null) return;
                
        Vector4 spawnRect = _spawnRects[Random.Range(0, _spawnRects.Count)];
        Vector3 spawnPos = new Vector3(Random.Range(spawnRect.x, spawnRect.z), Random.Range(spawnRect.y, spawnRect.w), 0f);
        enemy.Reset(spawnPos);
    }

    public void ChangeMaxActiveEnemies(int amount)
    {
        MAX_ACTIVE_ENEMIES = amount;
    }

    public void Reset(bool kill)
    {
        foreach (EnemyPool enemyPool in _enemyPools)
            enemyPool.Reset(kill);
    }
    
    public void AddInactiveEnemie(Enemy enemy, Enemy.EnemyType type)
    {
        foreach (EnemyPool enemyPool in _enemyPools)
        {
            if (enemyPool._type == type)
            {
                enemyPool._inactiveEnemies.Add(enemy);
                break;
            }
        }
    }

    public void RemoveInactiveEnemie(Enemy enemy, Enemy.EnemyType type)
    {
        foreach (EnemyPool enemyPool in _enemyPools)
        {
            if (enemyPool._type == type)
            {
                enemyPool._inactiveEnemies.Remove(enemy);
                break;
            }
        }
    }
}

[System.Serializable]
public class EnemyPool
{
    public Enemy.EnemyType _type;
    public GameObject _enemyParent;
    public List<Enemy> _enemies = new List<Enemy>();
    public List<Enemy> _inactiveEnemies = new List<Enemy>();

    public void Setup()
    {
        _enemies.Clear();
        _inactiveEnemies.Clear();

        _enemies.AddRange(_enemyParent.GetComponentsInChildren<Enemy>());
    }

    public void Reset(bool kill)
    {
        foreach (Enemy enemy in _enemies)
        {
            if (kill && enemy.gameObject.activeSelf)
            {
                enemy.Die();
                continue;
            }
            
            enemy.Reset(Vector3.zero);
            enemy.gameObject.SetActive(false);
        }
    }
}