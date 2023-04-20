using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBounds : MonoBehaviour
{
    public void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy enemy = other.GetComponent<Enemy>();
            if (enemy == null) enemy = other.transform.parent.GetComponent<Enemy>();
            enemy.HandleCollision(other);
            return;
        }

        if (other.CompareTag("Player"))
            Player.Instance.HandleCollision(other);
    }
}
