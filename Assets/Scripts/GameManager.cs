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
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
    }

    private void Update()
    {
        HandleReset();
        HandleMenu();
        HandleClosestEnemy();
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

    public void HandleClosestEnemy(){
        bool haveSlowDown=false;
        int slowDownPowerUpIndex=0;
        for (int i=0; i < Inventory.Instance.powerUps.Count; i++)
        {
            if (Inventory.Instance.powerUps[i].GetName() == "SlowDownPowerUp"){
                haveSlowDown = true;
                slowDownPowerUpIndex = i;
            }
        }
        if (Player.Instance.GetClosestEnemy()!=null){
            if ((Player.Instance.GetClosestEnemy().GetToPlayer().magnitude <= 1.5f)&(haveSlowDown)){
                Inventory.Instance.powerUps[slowDownPowerUpIndex].ActivatePowerUp();
                print(Inventory.Instance.powerUps[slowDownPowerUpIndex].cooldownTimer);
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
        yield return new WaitForSeconds(time);
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

    public void CallStopTime(){
        StartCoroutine(StopTime());
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

    public IEnumerator StopTime(){
        Time.timeScale = .001f;
        _shaderMaterial.SetFloat("_ChromaticAberration", 0.01f);
        yield return new WaitForSeconds(.001f);
        Player.Instance.isInvencible = false;
        yield return new WaitForSeconds(.001f);
        _shaderMaterial.SetFloat("_ChromaticAberration", 0f);
        Time.timeScale = 1;
    }
}
