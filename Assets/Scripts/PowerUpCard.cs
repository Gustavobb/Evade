using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCard : MonoBehaviour
{
    public GenericPowerUp powerUp;
    [SerializeField] AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnMouseOver(){
        // fazer algum efeito de hover
        if(Input.GetMouseButtonDown(0))
        {
            audioSource.Play();
            PowerUpManager.Instance.AddPowerUp(powerUp);
        }
    }  
}
