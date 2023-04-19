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

    [Header("Visuals")]
    [SerializeField] private bool _rotate = false;
    private bool _originalRotate;
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private float _rotationSpeed = 5f;

    private List<Shape> _shapes = new List<Shape>();
    private List<Color> _originalColors = new List<Color>();
    public bool IsInsideArena { get; private set; }

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        if (Player.Instance == null || Player.Instance.gameObject.activeSelf == false)
            return;

        if (_rotate) Rotate();
        HandleEnemyBehaviour();

        if (Player.Instance != null)
            Player.Instance.SetClosestEnemy(this);
    }

    private void Setup()
    {
        EnemyManager.Instance.AddEnemy(this);
        GetShapes();
        _originalRotate = _rotate;

        if (_rotationPivot == null)
            _rotationPivot = transform;
        
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

    public void ReflectVelocity()
    {
        _velocity *= -1;
        _toPlayerInitial *= -1;
    }

    public Vector3 GetToPlayer()
    {
        return _toPlayer;
    }

    public void SetInsideArena()
    {
        _velocity = Vector2.zero;
        _rotate = _originalRotate;
        
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpBlendBlack(0, 1, .5f);
        
        StartCoroutine(WaitToEnterArena(.5f));
    }

    private void Rotate()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, _rotationSpeed * Time.deltaTime);
        _rotationPivot.rotation *= rotation;
    }

    public void Reset(Vector3 position)
    {
        transform.position = position;
        _velocity = Vector3.zero;
        _rotate = false;
        IsInsideArena = false;
        
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

    IEnumerator WaitToEnterArena(float time)
    {
        yield return new WaitForSeconds(time);
        IsInsideArena = true;
    }

    public void HandleCollision(Collider2D other)
    {
        if (other.CompareTag("Player"))
            Player.Instance.Die();
    }
}
