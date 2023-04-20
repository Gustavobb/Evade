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
    
    [SerializeField] private Material _shaderMaterial;
    private bool onGame;
    public bool OnGame => onGame;
    
    private void Start() {
        ActivateMenu();
    }

    private void Update()
    {
        HandleReset();
        HandleMenu();
        HandlePause();

        // Alterar condição para fim da wave
        if (Input.GetMouseButtonDown(1)){
            PowerUpManager.Instance.OpenChoiceMenu();
        }
    }

    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pauseUI.SetActive(!pauseUI.activeSelf);
            onGame = !onGame;
        }
    }

    private void HandleReset()
    {
        if (Player.Instance == null) return;
        
        if (!onGame && !pauseUI.activeSelf)
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
        pauseUI.SetActive(false);
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

    public void CallSlowDown(){
        StartCoroutine(SlowDown());
    }
    
    public IEnumerator SlowDown(){
        Time.timeScale = .1f;
        _shaderMaterial.SetFloat("_ChromaticAberration", 0.01f);
        yield return new WaitForSeconds(.1f);
        Player.Instance.isInvencible = false;
        yield return new WaitForSeconds(.2f);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
        Time.timeScale = 1;
    }
}
