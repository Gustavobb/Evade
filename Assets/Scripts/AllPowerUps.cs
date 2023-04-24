using UnityEngine;

[System.Serializable]
public class SizeUpPowerUp: GenericPowerUp{

    public SizeUpPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        Player.Instance.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }
}

[System.Serializable]
public class LifeUpPowerUp: GenericPowerUp{

    public LifeUpPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        Player.Instance.lifes ++;
    }
}

[System.Serializable]
public class SlowDownPowerUp: GenericPowerUp{

    public SlowDownPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        this.cooldown = 11 - quantity*1;
    }

    public override void ActivatePowerUp(){
        if(this.cooldownTimer >= WaveManager.Instance._waveTime+cooldown){
            GameManager.Instance.CallSlowDown();
            this.cooldownTimer = WaveManager.Instance._waveTime;
        }
    }
}