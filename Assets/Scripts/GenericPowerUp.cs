[System.Serializable]
public class GenericPowerUp{
    public string name;
    public bool isActive;
    public int quantity;

    public GenericPowerUp(string name, bool isActive, int quantity){
        this.name = name;
        this.isActive = isActive;
        this.quantity = quantity;
    }

    public virtual void ObtainPowerUp(){
        quantity ++;
        // efeito quando recebe o powerup
    }

    public virtual string GetName(){
        return name;
    }
}

[System.Serializable]
public class ConsumablePowerUp: GenericPowerUp{
    public float cooldown;
    public ConsumablePowerUp(string name, bool isActive, int quantity, float cooldown):base(name, isActive, quantity){
        this.cooldown = cooldown;
    }

    public virtual void CheckCooldown(){
        if (cooldown <= 0) ActivatePowerUp();
    }
    
    public override void ObtainPowerUp(){ 
        base.ObtainPowerUp();
        cooldown = 0;
        // efeito quando recebe o powerup consumivel
    }

    public virtual void ActivatePowerUp(){
        // efeito quando ativa um powerup que o cooldown acabou
    }

    public override string GetName(){
        return name;
    }
}