public class GenericPowerUp{
    public string name;
    public bool isActive;
    public int quantity;

    public GenericPowerUp(){
        this.name = name;
        this.isActive = isActive;
        this.quantity = quantity;
    }

    public virtual void ObtainPowerUp(){
        quantity ++;
        // efeito quando recebe o powerup
    }
}

public class ConsumablePowerUp: GenericPowerUp{
    public float cooldown;

    public void CheckCooldown(){
        if (cooldown <= 0) ActivatePowerUp();
    }
    
    public override void ObtainPowerUp(){ 
        base.ObtainPowerUp();
        cooldown = 0;
        // efeito quando recebe o powerup consumivel
    }

    public void ActivatePowerUp(){// efeito quando ativa um powerup que o cooldown acabou
    }
}