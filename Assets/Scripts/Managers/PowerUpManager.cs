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
    [SerializeField] private bool _onPowerUpMenu;
    public bool OnPowerUpMenu => _onPowerUpMenu;
    [SerializeField] private List<Shape> _shapes = new List<Shape>();
    
    private void Start()
    {
        allPowerUps.Add(new LifeUpPowerUp("LifeUpPowerUp", true, 0, 0 ,0));
        allPowerUps.Add(new SizeUpPowerUp("SizeUpPowerUp", true, 0, 0, 0));
        allPowerUps.Add(new AddClockPowerUp("AddClockPowerUp", true, 0, 0, 0));
        // allPowerUps.Add(new SlowDownPowerUp("SlowDownPowerUp", true, 1, 10, 100));

        GetAllShapesFromChildren();
    }

    public void addPowerUp(GenericPowerUp powerUp){
        powerUp.ObtainPowerUp();
        // for (int i=0; i < allPowerUps.Count; i++)
        // {
        //     if (allPowerUps[i].GetName() == powerUp.GetName()){
        //         allPowerUps[i].ObtainPowerUp();
        //     }
        // }
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
            foreach (Transform child in transform)
                child.gameObject.SetActive(false);
            _onPowerUpMenu = false;
            GameManager.Instance.StopPowerUpPP();
            AudioHelper.Instance.SmoothLowPass(1f, .5f);
        }
    }

    public void OpenChoiceMenu()
    {
        AudioHelper.Instance.SmoothLowPass(0.1f, .5f);

        List<GenericPowerUp> possiblePowerUps = new List<GenericPowerUp>();
        print(allPowerUps.Count);
        for (int i = 0; i < allPowerUps.Count; i++)
        {
            print(allPowerUps[i].CheckCondition());
            if(allPowerUps[i].CheckCondition()) possiblePowerUps.Add(allPowerUps[i]);
        }

        _onPowerUpMenu = true;
        Cursor.visible = true;
        transform.position = new Vector3(_xAnimation, transform.position.y, transform.position.z);
        if(possiblePowerUps.Count != 0){
        // fazer inimigos sumirem
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);
                child.gameObject.SetActive(true);

                // _shapes[i].LerpAlpha(0, 1, .1f);
                PowerUpCard powerUpCard = child.gameObject.GetComponent<PowerUpCard>();

                if (powerUpCard == null) continue;
                int index = Random.Range(0, possiblePowerUps.Count);
                print(possiblePowerUps[0]);
                powerUpCard.powerUp = possiblePowerUps[index];
            }
        }
        StartCoroutine(MenuAnimation(.5f, _xAnimation, 0, true));
    }

    public void SelectPowerUp(GenericPowerUp powerUp)
    {
        addPowerUp(powerUp);
        PowerUpManager.Instance.CloseChoiceMenu();
    }

    public void CloseChoiceMenu()
    {
        Cursor.visible = false;
        WaveManager.Instance.SetupWave();
        StartCoroutine(MenuAnimation(.5f, 0, _xAnimation, false));
    }
}
