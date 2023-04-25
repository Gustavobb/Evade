using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "ScriptableObjects/EnemyData", order = 1)]
public class EnemyData : ScriptableObject
{
    public float speedMultiplier = 1f;
    public float sizeMultiplier = 1f;
    public float speedMultiplierPowerUp = 1f;
    public float sizeMultiplierPowerUp = 1f;

    public void MultiplySpeed(float multiplier)
    {
        speedMultiplier = multiplier;
    }

    public void MultiplySize(float multiplier)
    {
        sizeMultiplier = multiplier;
    }

    public void MultiplySpeedPowerUp(float multiplier)
    {
        speedMultiplierPowerUp -= multiplier;
    }

    public void MultiplySizePowerUp(float multiplier)
    {
        sizeMultiplierPowerUp -= multiplier;
    }

    public void Reset()
    {
        speedMultiplier = 1f;
        sizeMultiplier = 1f;
        speedMultiplierPowerUp = 1f;
        sizeMultiplierPowerUp = 1f;
    }
}