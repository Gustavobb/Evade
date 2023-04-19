using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBounds : MonoBehaviour
{
    public void EnterArena(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.Die();

        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) enemy = other.transform.parent.GetComponent<Enemy>();

            if (!enemy.IsInsideArena)
                enemy.SetInsideArena();
            
            else if (enemy.IsInsideArena)
            {
                if (enemy._behaviour == Enemy.Behaviour.Bouncer)
                {
                    enemy.ReflectVelocity();
                    return;
                }

                EnemyManager.Instance.KillEnemy(enemy);
            }
        }
    }
}
