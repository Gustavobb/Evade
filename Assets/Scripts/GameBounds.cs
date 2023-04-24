using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBounds : MonoBehaviour
{
    private static GameBounds _instance;
    public static GameBounds Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameBounds>();
            return _instance;
        }
    }
    private AudioSource _audioSource;

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }
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

    public void PlaySound()
    {
        _audioSource.pitch = Random.Range(0.8f, 1.2f);
        _audioSource.Play();
    }
}
