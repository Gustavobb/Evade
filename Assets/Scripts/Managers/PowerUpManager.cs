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
    public List<GenericPowerUp> allPowerUps = new List<GenericPowerUp>();
    public List<PowerUpCard> powerUpCards = new List<PowerUpCard>();
    [SerializeField] private bool _onPowerUpMenu;
    public bool OnPowerUpMenu => _onPowerUpMenu;
    [SerializeField] private List<Shape> _shapes = new List<Shape>();
    public List<GameObject> noPowerUpPrefabs = new List<GameObject>();
    
    private void Start()
    {
        allPowerUps.Add(new LifeUpPowerUp("LifeUpPowerUp", true, 0, 0 ,0));
        allPowerUps.Add(new SizeDownPowerUp("SizeDownPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new AddClockPowerUp("AddClockPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new EnemySpeedDownPowerUp("EnemySpeedDownPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new EnemySizeDownPowerUp("EnemySizeDownPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new GuardianPowerUp("GuardianPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new GuardianSizeUpPowerUp("GuardianSizeUpPowerUp", true, 0, 0, 0));

        for (int i = 0; i < transform.childCount; i++)
        {
            PowerUpCard powerUpCard = transform.GetChild(i).GetComponent<PowerUpCard>();

            if (powerUpCard != null)
                powerUpCards.Add(powerUpCard);
       }

        foreach (GameObject noPowerUpPrefab in noPowerUpPrefabs)
            noPowerUpPrefab.SetActive(false);

        GetAllShapesFromChildren();
    }

    public void AddPowerUp(GenericPowerUp powerUp){
        if (powerUp != null)
            powerUp.ObtainPowerUp();
        
        PowerUpManager.Instance.CloseChoiceMenu();
    }

    private void GetAllShapesFromChildren()
    {
        foreach (Transform child in transform)
            _shapes.Add(child.gameObject.GetComponent<Shape>());
    }

    private IEnumerator MenuAnimation(float time, float start, float end, bool open)
    {
        float elapsedTime = 0;
        GameManager.Instance.RequestPowerUpPP();

        while (elapsedTime < time)
        {
            transform.position = new Vector3(Mathf.Lerp(start, end, elapsedTime / time), transform.position.y, transform.position.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = new Vector3(end, transform.position.y, transform.position.z);

        if (!open)
        {
            foreach (Transform child in transform){
                child.gameObject.SetActive(false);
                Transform icon;
                if (child.gameObject.transform.childCount > 0){
                    icon = child.gameObject.transform.GetChild(0);
                    icon.parent = null;
                    icon.gameObject.SetActive(false);
                }
            }
            _onPowerUpMenu = false;
            GameManager.Instance.StopPowerUpPP();
            AudioHelper.Instance.SmoothLowPass(1f, .5f);
        }
    }

    public void OpenChoiceMenu()
    {
        AudioHelper.Instance.SmoothLowPass(0.1f, .5f);

        List<GenericPowerUp> possiblePowerUps = new List<GenericPowerUp>();
        for (int i = 0; i < allPowerUps.Count; i++)
            if(allPowerUps[i].CheckCondition()) possiblePowerUps.Add(allPowerUps[i]);

        _onPowerUpMenu = true;
        Cursor.visible = true;
        transform.position = new Vector3(_xAnimation, transform.position.y, transform.position.z);
        transform.GetChild(0).gameObject.SetActive(true);

        for (int i = 0; i < powerUpCards.Count; i++)
        {
            Transform child = powerUpCards[i].transform;
            child.gameObject.SetActive(true);

            PowerUpCard powerUpCard = powerUpCards[i];

            GameObject icon;
            icon = noPowerUpPrefabs[i];
            if (possiblePowerUps.Count > 0)
            {
                int index = Random.Range(0, possiblePowerUps.Count);
                powerUpCard.powerUp = possiblePowerUps[index];
                icon = possiblePowerUps[index].icon;
                possiblePowerUps.RemoveAt(index);
            }

            icon.transform.position = child.transform.position;
            icon.transform.parent = child.transform;
            icon.SetActive(true);
        }
        StartCoroutine(MenuAnimation(.5f, _xAnimation, 0, true));
    }

    public void CloseChoiceMenu()
    {
        Cursor.visible = false;
        WaveManager.Instance.SetupWave();
        StartCoroutine(MenuAnimation(.5f, 0, _xAnimation, false));
    }
}
