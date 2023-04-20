using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private static Player _instance;
    public static Player Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }

    private Enemy _closestEnemy;

    private void Update()
    {
        if (!GameManager.Instance.OnGame) return;
        
        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
        {
            Cursor.visible = true;
            return;
        }

        Cursor.visible = false;
        transform.position = Camera.main.ScreenToWorldPoint(mousePos);
    }

    public void SetClosestEnemy(Enemy enemy)
    {
        if (_closestEnemy == null)
        {
            _closestEnemy = enemy;
            return;
        }

        if (enemy.GetToPlayer().magnitude < _closestEnemy.GetToPlayer().magnitude)
            _closestEnemy = enemy;
    }

    public void Reset()
    {
        gameObject.SetActive(true);
        transform.position = Vector3.zero;
    }

    public void Die()
    {
        gameObject.SetActive(false);
        Cursor.visible = true;
        GameManager.Instance.GameOver();
    }

    public void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
            Die();
        
        if (other.CompareTag("Bounds"))
            Die();
    }
}
