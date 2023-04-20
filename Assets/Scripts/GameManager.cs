using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<GameManager>();
            return _instance;
        }
    }

    [SerializeField] private GameObject menuUI, pauseUI;
    private bool onGame;
    public bool OnGame => onGame;
    
    private void Start() {
        ActivateMenu();
    }

    private void Update()
    {
        HandlePause();
        HandleReset();
        HandleMenu();
    }

    private void HandlePause()
    {
        if (!onGame)
        {
            if (Input.GetMouseButtonDown(0))
                pauseUI.SetActive(false);

            return;
        } 

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(!pauseUI.activeSelf);
            onGame = !onGame;
        }
    }

    private void HandleReset()
    {
        if (Player.Instance == null) return;
        
        if (!Player.Instance.gameObject.activeSelf)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Player.Instance.Reset();
                EnemyManager.Instance.Reset();
            }
        }
    }

    private void HandleMenu()
    {
        if (!onGame & Input.GetMouseButtonDown(0)){
            DestroyMenu();
        }
    }

    private void DestroyMenu()
    {
        menuUI.SetActive(false);
        onGame = true;
    }

    protected void ActivateMenu()
    {
        menuUI.SetActive(true);
        pauseUI.SetActive(false);
        onGame = false;
    }

    public void GameOver()
    {
        ActivateMenu();
        // matar inimigos
        // resetar power ups
    }
}
