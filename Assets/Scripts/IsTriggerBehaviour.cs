using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent<Collider2D> _onTriggerEnter, _onTriggerExit;
    private Collider2D _collider2D;

    private void Awake()
    {
        _collider2D = GetComponent<Collider2D>();
        _collider2D.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        _onTriggerEnter?.Invoke(other);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _onTriggerExit?.Invoke(other);
    }
}
