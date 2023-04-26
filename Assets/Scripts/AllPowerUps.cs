using UnityEngine;

[System.Serializable]
public class SizeDownPowerUp: GenericPowerUp{

    public SizeDownPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        Player.Instance.transform.localScale -= new Vector3(0.1f, 0.1f, 0.1f);
    }

    public override bool CheckCondition(){
        return (Player.Instance.transform.localScale.x >= .3f);
    }
}

[System.Serializable]
public class LifeUpPowerUp: GenericPowerUp{

    public LifeUpPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        LifesManager.Instance.GainLife();
    }

    public override bool CheckCondition(){
        return LifesManager.Instance._lifesCount < LifesManager.Instance.MAX_LIFES;
    }
}

[System.Serializable]
public class AddClockPowerUp: GenericPowerUp{

    public AddClockPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        ClockManager.Instance._clockCount++;
    }

    public override bool CheckCondition(){
        return ClockManager.Instance._clockCount < ClockManager.Instance.MAX_CLOCKS;
    }
}


[System.Serializable]
public class EnemySpeedDownPowerUp: GenericPowerUp{

    public EnemySpeedDownPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        EnemyManager.Instance.enemyData.MultiplySpeedPowerUp(0.1f);
    }

    public override bool CheckCondition(){
        return EnemyManager.Instance.enemyData.speedMultiplierPowerUp > .5f;
    }
}

[System.Serializable]
public class EnemySizeDownPowerUp: GenericPowerUp{

    public EnemySizeDownPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        EnemyManager.Instance.enemyData.MultiplySizePowerUp(0.05f);
    }

    public override bool CheckCondition(){
        return EnemyManager.Instance.enemyData.sizeMultiplierPowerUp > .3f;
    }
}

[System.Serializable]
public class GuardianPowerUp: GenericPowerUp{

    public GuardianPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        GuardianManager.Instance.AddGuardian();
    }

    public override bool CheckCondition(){
        return GuardianManager.Instance._guardianCount < GuardianManager.Instance.MAX_GUARDIANS;
    }
}

[System.Serializable]
public class GuardianSizeUpPowerUp: GenericPowerUp{

    public GuardianSizeUpPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer):base(name, isActive, quantity, cooldown, cooldownTimer){}

    public override void ObtainPowerUp(){
        base.ObtainPowerUp();
        GuardianManager.Instance.guardianData.MultiplySizePowerUp(0.05f);
    }

    public override bool CheckCondition(){
        return GuardianManager.Instance.guardianData.sizeMultiplierPowerUp < 1.7f && GuardianManager.Instance._guardianCount > 0;
    }
}