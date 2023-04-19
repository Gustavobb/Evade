using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class IsTriggerBehaviour : MonoBehaviour
{
    [SerializeField] private UnityEvent _onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            print("Player hit");
            GameManager.Instance.GameOver();
        _onTriggerEnter?.Invoke();
    }
}
