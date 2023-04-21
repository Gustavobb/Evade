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
    [SerializeField] private bool _onPowerUpMenu;
    public bool OnPowerUpMenu => _onPowerUpMenu;
    [SerializeField] private List<Shape> _shapes = new List<Shape>();
    
    // Start is called before the first frame update
    void Start()
    {
        allPowerUps.Add(new LifeUpPowerUp("LifeUpPowerUp", true, 1));
        allPowerUps.Add(new SizeUpPowerUp("SizeUpPowerUp", true, 1));
        GetAllShapesFromChildren();
    }

    private void GetAllShapesFromChildren(){
        foreach (Transform child in transform){
            _shapes.Add(child.gameObject.GetComponent<Shape>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator OpenChoiceMenuCoroutine(float time){
        _onPowerUpMenu = true;
        Player.Instance.EnableCollider(false);
        yield return new WaitForSeconds(time);
        Cursor.visible = true;
        Player.Instance.gameObject.SetActive(false);
        // fazer inimigos sumirem
        for (int i = 0; i < transform.childCount; i++){
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);

            _shapes[i].LerpAlpha(0, 1, .1f);
            int index = Random.Range(0, allPowerUps.Count);
            child.gameObject.GetComponent<PowerUpCard>().powerUp = allPowerUps[index];
        }
    }

    public void OpenChoiceMenu(){
        StartCoroutine(OpenChoiceMenuCoroutine(.25f));
    }

    public void SelectPowerUp(GenericPowerUp powerUp){
        Inventory.Instance.addPowerUp(powerUp);
        PowerUpManager.Instance.CloseChoiceMenu();
    }

    public void CloseChoiceMenu(){
        Player.Instance.EnableCollider(true);
        Player.Instance.Reset();
        Cursor.visible = false;
        _onPowerUpMenu = false;
        foreach (Transform child in transform){
            child.gameObject.SetActive(false);
        }
    }
}
