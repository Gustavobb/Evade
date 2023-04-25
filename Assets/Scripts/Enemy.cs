using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum Behaviour
    {
        FollowPlayer,
        AvoidPlayer,
        Wander,
        PlayerDirection,
        Bouncer,
        DoNothing
    }

    public enum EnemyType
    {
        Square,
        Rectangle,
        Triangle,
        Circle,
        Polygon,
        Star,
        Arrow
    }

    [Header("Type")]
    public EnemyType _type;

    [Header("Behaviour")]
    public Behaviour _behaviour;
    private Vector2 _velocity, _toPlayerInitial, _toPlayer;

    [Header("Movement")]
    [SerializeField] private Transform _movementPivot;
    [SerializeField] private float MAX_SPEED = 5f;
    [SerializeField] private float _speed = 5f;

    private List<Shape> _shapes = new List<Shape>();
    private List<Color> _originalColors = new List<Color>();
    private bool _isInsideArena;
    [SerializeField] private Rotate _rotate;
    [SerializeField] private float _timeToEnterArena = .5f;
    [SerializeField] private float _timeToDie = .5f;
    private bool _dead = false;
    [SerializeField] private bool _reportToManager = true;
    [SerializeField] private bool _needsToEnterArena = true;
    private Collider2D _collider;
    [SerializeField] private EnemyData _enemyData;
    private Vector3 _originalScale;

    private void Start()
    {
        _originalScale = transform.localScale;
        Setup();
    }

    private void Update()
    {
        if (!GameManager.Instance.OnGame || Pause.Paused || _dead || PowerUpManager.Instance.OnPowerUpMenu || Player.Instance.IsInvincible) return;

        if (Player.Instance == null || Player.Instance.gameObject.activeSelf == false)
            return;

        HandleEnemyBehaviour();

        if (Player.Instance != null)
            Player.Instance.SetClosestEnemy(this);
    }

    private void Setup()
    {
        _collider = GetComponent<Collider2D>();
        _rotate = GetComponent<Rotate>();

        GetShapes();

        if (_movementPivot == null)
            _movementPivot = transform;
        
        Reset(transform.position);
    }

    private void GetShapes()
    {
        _shapes.Clear();
        _originalColors.Clear();
        _shapes.AddRange(GetComponentsInChildren<Shape>());
        _shapes.AddRange(GetComponents<Shape>());

        foreach (Shape shape in _shapes)
            _originalColors.Add(shape.GetColor());
    }

    public void SetColor(Color color, Shape.ColorType colorType)
    {
        foreach (Shape shape in _shapes)
        {
            shape.SetColor(color);
            shape.SetColorType(colorType);
        }
    }

    private void HandleEnemyBehaviour()
    {
        if (Player.Instance != null)
            _toPlayer = Player.Instance.transform.position - transform.position;

        switch (_behaviour)
        {
            case Behaviour.FollowPlayer:
                FollowPlayer();
                break;
            case Behaviour.AvoidPlayer:
                AvoidPlayer();
                break;
            case Behaviour.Wander:
                Wander();
                break;
            case Behaviour.PlayerDirection:
                PlayerDirection();
                break;
            case Behaviour.Bouncer:
                Bouncer();
                break;
            case Behaviour.DoNothing:
                _velocity = Vector3.zero;
                break;
        }

        _velocity = Vector3.ClampMagnitude(_velocity, MAX_SPEED * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp);
        _movementPivot.position += new Vector3(_velocity.x, _velocity.y, 0) * Time.deltaTime;
    }

    private void FollowPlayer()
    {
        _velocity += _toPlayer.normalized * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp * Time.deltaTime;
    }

    private void AvoidPlayer()
    {
        _velocity -= _toPlayer.normalized * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp * Time.deltaTime;
    }

    private void Wander()
    {
        _velocity += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp * Time.deltaTime;
    }
    
    private void PlayerDirection()
    {
        _velocity += _toPlayerInitial.normalized * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp * Time.deltaTime;
        transform.rotation = Quaternion.LookRotation(Vector3.forward, _toPlayerInitial);
    }

    private void Bouncer()
    {
        _velocity += _toPlayerInitial.normalized * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp * Time.deltaTime;
    }

    private void BounceToPlayer()
    {
        float angle = Mathf.Atan2(_toPlayer.y, _toPlayer.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _velocity = rotation * Vector2.right * _speed * _enemyData.speedMultiplier * _enemyData.speedMultiplierPowerUp;
        _toPlayerInitial = _toPlayer;
    }

    public Vector3 GetToPlayer()
    {
        return _toPlayer;
    }

    public void SetInsideArena()
    {
        StartCoroutine(WaitToEnterArena());
        _velocity = Vector2.zero;
        
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpBlendBlack(0, 1, _timeToEnterArena);

        _isInsideArena = true;
    }

    public void EnableCollider(bool enable)
    {
        _collider.enabled = enable;
    }
    
    public void Reset(Vector3 position)
    {
        transform.localScale = _originalScale * _enemyData.sizeMultiplier * _enemyData.sizeMultiplierPowerUp;
        transform.position = position;
        _velocity = Vector3.zero;
        _dead = false;
        _isInsideArena = !_needsToEnterArena;
        if (_rotate != null && _needsToEnterArena) _rotate.enabled = false;
        
        for (int i = 0; i < _shapes.Count; i++)
        {
            if (_needsToEnterArena) _shapes[i].SetBlendBlack(0);
            _shapes[i].ResetAlpha();
        }

        if (Player.Instance != null)
            _toPlayerInitial = Player.Instance.transform.position - transform.position;
    }

    private IEnumerator WaitToEnterArena()
    {
        yield return new WaitForSeconds(_timeToEnterArena);
        if (_rotate != null) 
        {
            _rotate.enabled = true;
            _rotate.LerpRotationSpeed();
        }
    }

    private IEnumerator DieCoroutine()
    {
        _dead = true;
        GameBounds.Instance.PlaySound();
        bool canWobble = !GameManager.Instance.wobbling;
        if (canWobble) GameManager.Instance.RequestWobble();

        _velocity = Vector2.zero;
        if (_rotate != null && _needsToEnterArena) _rotate.enabled = false;

        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpAlpha(1, 0, _timeToDie);

        yield return new WaitForSeconds(_timeToDie);
        if (canWobble) GameManager.Instance.StopWobble();
        Reset(Vector3.zero);
        gameObject.SetActive(false);
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    }

    public void SmoothAppear(float time)
    {
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpAlpha(0, 1, time);
    }

    private void OnDisable()
    {
        if (_reportToManager)
            EnemyManager.Instance.AddInactiveEnemie(this, _type);
    }

    private void OnEnable()
    {
        if (_reportToManager)
            EnemyManager.Instance.RemoveInactiveEnemie(this, _type);
    }

    private void HandleBoundsCollision(Collider2D other)
    {
        bool inside = true;
        Vector2 normal = (Vector2.zero - (Vector2) other.transform.position).normalized;

        Vector2 diff = transform.position - other.transform.position;
        float dot = Vector2.Dot(diff, normal);
        inside &= dot > 0;

        if (inside)
            Die();
    }

    public void HandleCollision(Collider2D other)
    {
        if (_dead) return;
        if (other.CompareTag("Bounds"))
        {
            if (!_isInsideArena)
                SetInsideArena();
            
            else if (_isInsideArena)
            {
                if (_behaviour == Behaviour.Bouncer)
                {
                    BounceToPlayer();
                    return;
                }

                HandleBoundsCollision(other);
            }
        }

        else if (other.CompareTag("Guardian"))
        {
            Guardian guardian = other.GetComponent<Guardian>();
            guardian.Hit();
            Die();
        }
    }
}
