using UnityEngine;

[System.Serializable]
public class SizeUpPowerUp: GenericPowerUp{

    public SizeUpPowerUp(string name, bool isActive, int quantity):base(name, isActive, quantity){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        Player.Instance.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
}

[System.Serializable]
public class LifeUpPowerUp: GenericPowerUp{

    public LifeUpPowerUp(string name, bool isActive, int quantity):base(name, isActive, quantity){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        Player.Instance.lifes ++;
    }
}
