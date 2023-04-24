[System.Serializable]
public class GenericPowerUp{
    public string name;
    public bool isActive;
    public int quantity;
    public float cooldown, cooldownTimer;

    public GenericPowerUp(string name, bool isActive, int quantity, float cooldown, float cooldownTimer){
        this.name = name;
        this.isActive = isActive;
        this.quantity = quantity;
        this.cooldown = cooldown;
        this.cooldownTimer = cooldownTimer;
    }

    public virtual void ObtainPowerUp(){
        quantity ++;
    }

    public virtual void ActivatePowerUp(){
    }

    public virtual string GetName(){
        return name;
    }

    public virtual bool CheckCondition(){
        return true;
    }
}