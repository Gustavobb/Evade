using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private UnityEvent _onGameStart;
    [SerializeField] private GameObject menuUI;
    
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
    }

    private void HandleReset()
    {
        if (Player.Instance == null) return;
        
        if (!onGame)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Player.Instance.Reset();
                EnemyManager.Instance.Reset(false);
            }
        }
    }

    public void ShakeScreen(float time)
    {
        StartCoroutine(ShakeScreenCoroutine(time));
    }

    private IEnumerator ShakeScreenCoroutine(float time)
    {
        _shaderMaterial.SetFloat("_ScreenShake", 0.1f);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0.007f);
        yield return new WaitForSeconds(time);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
        _shaderMaterial.SetFloat("_ScreenShake", 0f);
    }

    private void HandleMenu()
    {
        if (!onGame & Input.GetMouseButtonDown(0)){
            DestroyMenu();
        }
    }

    private void DestroyMenu()
    {
        _onGameStart.Invoke();
        menuUI.SetActive(false);
        onGame = true;
    }

    protected void ActivateMenu()
    {
        menuUI.SetActive(true);
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

    public void RequestWobble()
    {
        if (!PowerUpManager.Instance.OnPowerUpMenu)
        {
            _shaderMaterial.SetFloat("_Wobble", 0.007f);
            _shaderMaterial.SetFloat("_OldTV", 0.007f);
        }
    }

    public void StopWobble()
    {
        _shaderMaterial.SetFloat("_Wobble", 0f);
        _shaderMaterial.SetFloat("_OldTV", 0f);
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
