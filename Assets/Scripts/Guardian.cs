using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardian : MonoBehaviour
{
    private Vector2 _velocity, _toPlayer;

    [Header("Movement")]
    [SerializeField] private Transform _movementPivot;
    [SerializeField] private float MAX_SPEED = 5f;
    [SerializeField] private float _speed = 5f;

    private List<Shape> _shapes = new List<Shape>();
    private List<Color> _originalColors = new List<Color>();
    [SerializeField] private Rotate _rotate;
    [SerializeField] private float _timeToDie = .5f;
    [SerializeField] private GuardianData _guardianData;

    private bool _dead = false;
    private Collider2D _collider;
    private Vector3 _originalScale;


    private void Awake()
    {
        Setup();
    }

    private void Update()
    {
        if (!GameManager.Instance.OnGame || Pause.Paused || _dead || PowerUpManager.Instance.OnPowerUpMenu || Player.Instance.IsInvincible) return;

        if (Player.Instance == null || Player.Instance.gameObject.activeSelf == false)
            return;

        HandleGuardianBehaviour();
    }

    private void Setup()
    {
        _collider = GetComponent<Collider2D>();
        _originalScale = transform.localScale;

        GetShapes();

        if (_movementPivot == null)
            _movementPivot = transform;
        
        Vector2 randomPointInCircle = Random.insideUnitCircle * 2f;
        Vector3 randomPoint = new Vector3(randomPointInCircle.x, randomPointInCircle.y, 0);
        Reset(randomPoint);
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

    private void HandleGuardianBehaviour()
    {
        if (Player.Instance != null)
            _toPlayer = Player.Instance.transform.position - transform.position;

        FollowPlayer();

        _velocity = Vector3.ClampMagnitude(_velocity, MAX_SPEED * _guardianData.speedMultiplier);
        _movementPivot.position += new Vector3(_velocity.x, _velocity.y, 0) * Time.deltaTime;
    }

    private void FollowPlayer()
    {
        _velocity += _toPlayer.normalized * _speed * _guardianData.speedMultiplier * Time.deltaTime;
    }

    public void EnableCollider(bool enable)
    {
        _collider.enabled = enable;
    }

    public void Hit()
    {
        _velocity = Vector2.zero;
    }
    
    public void Reset(Vector3 position)
    {
        transform.position = position;
        transform.localScale = _originalScale * _guardianData.sizeMultiplier;
        _velocity = Vector3.zero;
        _dead = false;
        MAX_SPEED = Random.Range(MAX_SPEED - 2f, MAX_SPEED + 2f);

        if (_rotate == null)
            _rotate = GetComponent<Rotate>();

        _rotate.enabled = true;
        
        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].ResetAlpha();
    }

    private IEnumerator DieCoroutine()
    {
        _dead = true;
        _velocity = Vector2.zero;

        if (_rotate == null)
            _rotate = GetComponent<Rotate>();
            
        _rotate.enabled = false;

        for (int i = 0; i < _shapes.Count; i++)
            _shapes[i].LerpAlpha(1, 0, _timeToDie);

        yield return new WaitForSeconds(_timeToDie);

        Vector2 randomPointInCircle = Random.insideUnitCircle * 2f;
        Vector3 randomPoint = new Vector3(randomPointInCircle.x, randomPointInCircle.y, 0);
        Reset(randomPoint);
        gameObject.SetActive(false);
    }

    public void Die()
    {
        StartCoroutine(DieCoroutine());
    }
}