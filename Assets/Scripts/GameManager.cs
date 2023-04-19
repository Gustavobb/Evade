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

    private GameObject menuUI;
    private bool onGame;
    
    private void Start() {
        menuUI = GameObject.Find("[Out]");
        ActivateMenu();
    }

    private void Update()
    {
        HandlePause();
        HandleMenu();
    }

    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    private void HandleMenu()
    {
        if (!onGame & Input.GetMouseButtonDown(0)){
            DestroyMenu();
        }
    }

    private void DestroyMenu(){
        menuUI.SetActive(false);
        Time.timeScale = 1;
        onGame = true;
    }

    protected void ActivateMenu(){
        menuUI.SetActive(true);
        Time.timeScale = 0;
        onGame = false;
    }

    public void GameOver(){
        ActivateMenu();
        // matar inimigos
        // resetar power ups
    }
}
