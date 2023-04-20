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
    
    [SerializeField] private Material _shaderMaterial;
    [SerializeField] private GameObject menuUI;
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

        // Alterar condição para fim da wave
        if (Input.GetMouseButtonDown(1)){
            PowerUpManager.Instance.OpenChoiceMenu();
        }
    }

    private void HandlePause()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Time.timeScale = Time.timeScale == 0 ? 1 : 0;
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
        Time.timeScale = 1;
        onGame = true;
    }

    protected void ActivateMenu()
    {
        menuUI.SetActive(true);
        Time.timeScale = 0;
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
