using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifesManager : MonoBehaviour
{
    public int MAX_LIFES = 5;
    public int _lifesCount = 0;

    private static LifesManager _instance;
    public static LifesManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<LifesManager>();
            return _instance;
        }
    }

    private List<GameObject> _lifes = new List<GameObject>();

    private void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            _lifes.Add(transform.GetChild(i).gameObject);
            if (i >= _lifesCount) _lifes[i].gameObject.SetActive(false);
        }
    }

    private void HandleLifes()
    {
        for (int i = 0; i < _lifes.Count; i++)
        {
            if (i >= _lifesCount) _lifes[i].gameObject.SetActive(false);
            else _lifes[i].gameObject.SetActive(true);
        }
    }

    public void LoseLife()
    {
        if (_lifesCount == 0) return;
        _lifesCount --;
        HandleLifes();
    }

    public void GainLife()
    {
        if (_lifesCount >= MAX_LIFES) return;
        _lifesCount ++;
        HandleLifes();
    }

    public void ResetLifes()
    {
        _lifesCount = 1;
        HandleLifes();
    }
}
