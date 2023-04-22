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

    [SerializeField] private float _xAnimation = 10f;
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

    private void GetAllShapesFromChildren()
    {
        foreach (Transform child in transform)
            _shapes.Add(child.gameObject.GetComponent<Shape>());
    }

    private IEnumerator OpenMenuAnimation(float time)
    {
        float elapsedTime = 0;
        float startX = transform.position.x;
        float endX = 0;

        while (elapsedTime < time)
        {
            transform.position = new Vector3(Mathf.Lerp(startX, endX, elapsedTime / time), transform.position.y, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(endX, transform.position.y, transform.position.z);
    }

    public void OpenChoiceMenu()
    {
        _onPowerUpMenu = true;
        Cursor.visible = true;
        transform.position = new Vector3(_xAnimation, transform.position.y, transform.position.z);
        
        // fazer inimigos sumirem
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            child.gameObject.SetActive(true);

            // _shapes[i].LerpAlpha(0, 1, .1f);
            PowerUpCard powerUpCard = child.gameObject.GetComponent<PowerUpCard>();

            if (powerUpCard == null) continue;
            int index = Random.Range(0, allPowerUps.Count);
            powerUpCard.powerUp = allPowerUps[index];
        }

        StartCoroutine(OpenMenuAnimation(.3f));
    }

    public void SelectPowerUp(GenericPowerUp powerUp)
    {
        Inventory.Instance.addPowerUp(powerUp);
        PowerUpManager.Instance.CloseChoiceMenu();
    }

    public void CloseChoiceMenu()
    {
        Player.Instance.Reset();
        Cursor.visible = false;
        _onPowerUpMenu = false;
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);

        WaveManager.Instance.SetupWave();
    }
}
