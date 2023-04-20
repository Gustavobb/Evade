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

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        if (!GameManager.Instance.OnGame) return;

        if (Player.Instance == null || Player.Instance.gameObject.activeSelf == false)
            return;

        HandleEnemyBehaviour();

        if (Player.Instance != null)
            Player.Instance.SetClosestEnemy(this);
    }

    private void Setup()
    {
        _rotate = GetComponent<Rotate>();
        EnemyManager.Instance.AddEnemy(this);
        GetShapes();

        if (_movementPivot == null)
            _movementPivot = transform;
        
        Reset(transform.position);
        gameObject.SetActive(false);
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

        _velocity = Vector3.ClampMagnitude(_velocity, MAX_SPEED);
        _movementPivot.position += new Vector3(_velocity.x, _velocity.y, 0) * Time.deltaTime;
    }

    private void FollowPlayer()
    {
        _velocity += _toPlayer.normalized * _speed * Time.deltaTime;
    }

    private void AvoidPlayer()
    {
        _velocity -= _toPlayer.normalized * _speed * Time.deltaTime;
    }

    private void Wander()
    {
        _velocity += new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * _speed * Time.deltaTime;
    }
    
    private void PlayerDirection()
    {
        _velocity += _toPlayerInitial.normalized * _speed * Time.deltaTime;
    }

    private void Bouncer()
    {
        _velocity += _toPlayerInitial.normalized * _speed * Time.deltaTime;
    }

    private void BounceToPlayer()
    {
        float angle = Mathf.Atan2(_toPlayer.y, _toPlayer.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        _velocity = rotation * Vector2.right * _speed;
        _toPlayerInitial = _toPlayer;
    }

    public Vector3 GetToPlayer()
    {
        return _toPlayer;
    }

    public void SetInsideArena()
    {
        _velocity = Vector2.zero;
        if (_rotate != null) _rotate.enabled = true;
        
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpBlendBlack(0, 1, .5f);

        _isInsideArena = true;
    }

    public void Reset(Vector3 position)
    {
        transform.position = position;
        _velocity = Vector3.zero;
        if (_rotate != null) _rotate.enabled = false;
        _isInsideArena = false;
        
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].SetBlendBlack(0);

        if (Player.Instance != null)
            _toPlayerInitial = Player.Instance.transform.position - transform.position;
    }

    private void OnDisable()
    {
        EnemyManager.Instance.AddInactiveEnemie(this);
    }

    private void OnEnable()
    {
        EnemyManager.Instance.RemoveInactiveEnemie(this);
    }

    private void OnDestroy()
    {
        EnemyManager.Instance.RemoveEnemy(this);
    }

    private void HandleBoundsCollision(Collider2D other)
    {
        bool inside = true;
        Vector2 normal = (Vector2.zero - (Vector2) other.transform.position).normalized;

        Vector2 diff = transform.position - other.transform.position;
        float dot = Vector2.Dot(diff, normal);
        inside &= dot > 0;

        if (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                diff = child.position - other.transform.position;
                dot = Vector2.Dot(diff, normal);

                inside &= dot > 0;
            }
        }

        if (inside)
            EnemyManager.Instance.KillEnemy(this);
    }

    public void HandleCollision(Collider2D other)
    {
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
    }
}
