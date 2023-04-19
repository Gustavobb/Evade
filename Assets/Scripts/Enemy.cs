using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private enum Behaviour
    {
        FollowPlayer,
        AvoidPlayer,
        Wander,
        PlayerDirection,
        DoNothing
    }

    [Header("Behaviour")]
    [SerializeField] private Behaviour _behaviour;
    private Vector3 _velocity, _toPlayerInitial, _toPlayer;

    [Header("Movement")]
    [SerializeField] private Transform _movementPivot;
    [SerializeField] private float MAX_SPEED = 5f;
    [SerializeField] private float _speed = 5f;

    [Header("Visuals")]
    [SerializeField] private bool _rotate = false;
    [SerializeField] private Transform _rotationPivot;
    [SerializeField] private float _rotationSpeed = 5f;

    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        if (_rotate) Rotate();
        HandleEnemyBehaviour();
        Player.Instance.SetClosestEnemy(this);
    }

    private void Setup()
    {
        EnemyManager.Instance.AddEnemy(this);

        if (_rotationPivot == null)
            _rotationPivot = transform;
        
        if (_movementPivot == null)
            _movementPivot = transform;
        
        _toPlayerInitial = Player.Instance.transform.position - transform.position;
    }

    private void HandleEnemyBehaviour()
    {
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
            case Behaviour.DoNothing:
                _velocity = Vector3.zero;
                break;
        }

        _velocity = Vector3.ClampMagnitude(_velocity, MAX_SPEED);
        _velocity.z = 0;
        _movementPivot.position += _velocity * Time.deltaTime;
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
        _velocity += new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0) * _speed * Time.deltaTime;
    }
    
    private void PlayerDirection()
    {
        _velocity += _toPlayerInitial.normalized * _speed * Time.deltaTime;
    }

    public Vector3 GetToPlayer()
    {
        return _toPlayer;
    }

    private void Rotate()
    {
        Quaternion rotation = Quaternion.Euler(0, 0, _rotationSpeed * Time.deltaTime);
        _rotationPivot.rotation *= rotation;
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
}
