using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField] private int MAX_ACTIVE_ENEMIES = 10;
    
    private List<Enemy> _enemiesPool = new List<Enemy>();
    private List<Enemy> _inactiveEnemies = new List<Enemy>();

    private static EnemyManager _instance;
    public static EnemyManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<EnemyManager>();
            return _instance;
        }
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
    
    public void AddInactiveEnemie(Enemy enemy)
    {
        _inactiveEnemies.Add(enemy);
    }

    public void RemoveInactiveEnemie(Enemy enemy)
    {
        _inactiveEnemies.Remove(enemy);
    }
}
