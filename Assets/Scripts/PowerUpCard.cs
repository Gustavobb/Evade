using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUpCard : MonoBehaviour
{
    public GenericPowerUp powerUp;

    private void OnMouseOver(){
        // fazer algum efeito de hover
        if(Input.GetMouseButtonDown(0)){
            PowerUpManager.Instance.SelectPowerUp(powerUp);
        }
    }  
}
