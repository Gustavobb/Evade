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
    [SerializeField] private int _invencibilityCycles = 10;
    [SerializeField] private float _invencibilityTime = 1f;

    private Collider2D _collider;
    private Shape _shape;
    private bool _isInvincible = false;
    public bool IsInvincible => _isInvincible;
    public Shape Shape => _shape;
    private AudioSource _audioSource;

    private void Start()
    {
        _collider = GetComponent<Collider2D>();
        _shape = GetComponent<Shape>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (!GameManager.Instance.OnGame || Pause.Paused) return;

        Vector2 mousePos = Input.mousePosition;
        if (mousePos.x < 0 || mousePos.x > Screen.width || mousePos.y < 0 || mousePos.y > Screen.height)
        {
            Cursor.visible = true;
            return;
        }

        _collider.enabled = !PowerUpManager.Instance.OnPowerUpMenu;
        Cursor.visible = PowerUpManager.Instance.OnPowerUpMenu;
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

    public Enemy GetClosestEnemy(){
        return _closestEnemy;
    }


    public void Reset()
    {
        gameObject.SetActive(true);
        transform.position = Vector3.zero;
    }

    public void Die()
    {
        // gameObject.SetActive(false);
        Cursor.visible = true;
        GameManager.Instance.GameOver();
    }

    public void EnableCollider(bool enable)
    {
        _collider.enabled = enable;
    }

    private IEnumerator Invincible()
    {
        _isInvincible = true;
        int cycles = 0;
        Color color = _shape.GetColor();
        GameManager.Instance.AnimateMaterial("_GreyScale", 0f, 1f, .3f);
        GameManager.Instance.AnimateMaterial("_ColorDecay", 1f, .2f, .3f);
        AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, 0, .5f, false, false);
        GameManager.Instance.SlowDown();

        while (cycles <= _invencibilityCycles)
        {
            if (!Pause.Paused)
            {
                color.a = cycles % 2 == 0 ? 0.2f : 1f;
                _shape.SetColor(color);
                cycles++;
                yield return new WaitForSeconds(_invencibilityTime / _invencibilityCycles);
            }

            yield return null;
        }

        color.a = 1f;
        GameManager.Instance.StopSlowDown();
        GameManager.Instance.AnimateMaterial("_GreyScale", 1f, 0f, .3f);
        GameManager.Instance.AnimateMaterial("_ColorDecay", .2f, 1f, .3f);
        AudioHelper.Instance.SmoothAudio(GameManager.Instance._AudioSource, 1, .5f, false, false);
        _shape.SetColor(color);
        _isInvincible = false;
    }

    public void HandleCollision(Collider2D other)
    {
        if (_isInvincible) return;
        if (other.CompareTag("Enemy"))
        {
            _audioSource.pitch = Random.Range(.9f, 1.1f);
            _audioSource.Play();
            GameManager.Instance.ShakeScreen(.15f);
            
            if (LifesManager.Instance._lifesCount <= 0) 
            {
                Die();
                return;
            }

            LifesManager.Instance.LoseLife();
            StartCoroutine(Invincible());
        }
        
        if (other.CompareTag("Bounds"))
        {
            _audioSource.pitch = Random.Range(.9f, 1.1f);
            _audioSource.Play();
            GameManager.Instance.ShakeScreen(.15f);

            if (LifesManager.Instance._lifesCount <= 0) 
            {
                Die();
                return;
            }

            LifesManager.Instance.LoseLife();
            StartCoroutine(Invincible());
        }
    }
}
