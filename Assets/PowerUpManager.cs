using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpManager : MonoBehaviour
{
    private static PowerUpManager _instance;
    public static PowerUpManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<PowerUpManager>();
            return _instance;
        }
    }

    private List<GenericPowerUp> allPowerUps = new List<GenericPowerUp>();
    
    // Start is called before the first frame update
    void Start()
    {
        allPowerUps.Add(new LifeUpPowerUp("LifeUpPowerUp", true, 1));
        allPowerUps.Add(new SizeUpPowerUp("SizeUpPowerUp", true, 1));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenChoiceMenu(){
        Time.timeScale = 0f;
        // fazer inimigos sumirem
        foreach (Transform child in transform){
            child.gameObject.SetActive(true);
            int index = Random.Range(0, allPowerUps.Count);
            child.gameObject.GetComponent<PowerUpCard>().powerUp = allPowerUps[index];
        }
    }

    public void SelectPowerUp(GenericPowerUp powerUp){
        Inventory.Instance.addPowerUp(powerUp);
        PowerUpManager.Instance.CloseChoiceMenu();
    }

    public void CloseChoiceMenu(){
        Time.timeScale = 1f;
        foreach (Transform child in transform){
            child.gameObject.SetActive(false);
        }
    }
}
