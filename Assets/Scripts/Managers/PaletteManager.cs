using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteManager : MonoBehaviour
{
    [SerializeField] private Material shaderMaterial;
    public List<Palette> palettes = new List<Palette>();

    private static PaletteManager instance;
    public static PaletteManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<PaletteManager>();
            }
            return instance;
        }
    }

    public void ApplyPalette(int idx)
    {
        if (idx < 0 || idx >= palettes.Count)
        {
            Debug.LogError("Invalid palette index");
            return;
        }

        Palette palette = palettes[idx];
        palette.ApplyPalette(shaderMaterial);
    }

    private void OnApplicationQuit()
    {
        Palette palette = palettes[palettes.Count - 1];
        palette.ApplyPalette(shaderMaterial);
    }
}

[System.Serializable]
public class Palette
{
    public string name;
    public bool useHueShift; 
    public bool randomHueShift;
    public float hueShift;
    public bool randomBackgroundColor;
    public Color backgroundColor;

    public List<EnemyPalette> enemyPalettes = new List<EnemyPalette>();
    public List<ShapePalette> shapePalettes = new List<ShapePalette>();

    public List<Guardian> guardians = new List<Guardian>();

    public bool randomPlayerColor;
    public Color playerColor;

    public void ApplyPalette(Material shaderMaterial)
    {
        if(useHueShift){
            shaderMaterial.SetFloat("_HueShift", hueShift);
            if(randomHueShift){
                float random = Random.Range(0f,1f);
                shaderMaterial.SetFloat("_HueShift", random);
            }
        }
        if (randomPlayerColor) Player.Instance.Shape.SetColor(Random.ColorHSV());
        else Player.Instance.Shape.SetColor(playerColor);

        if (randomBackgroundColor) shaderMaterial.SetColor("_Background", Random.ColorHSV());
        else shaderMaterial.SetColor("_Background", backgroundColor);

        foreach (EnemyPalette enemyPalette in enemyPalettes)
            enemyPalette.ApplyPalette(shaderMaterial);

        foreach (ShapePalette shapePalette in shapePalettes)
            shapePalette.ApplyPalette(shaderMaterial);
        

        foreach (Guardian guardian in guardians)
            guardian.SetColor(Player.Instance.Shape.GetColor(), (Shape.ColorType) Player.Instance.Shape.GetColorType());
    }
}

[System.Serializable]
public class EnemyPalette
{
    public Enemy.EnemyType enemyType;
    public Shape.ColorType colorType;
    public bool randomColor;
    public Color color;

    public void ApplyPalette(Material shaderMaterial)
    {
        foreach (EnemyPool pool in EnemyManager.Instance._enemyPools)
        {
            if(pool._type == enemyType) {
                foreach (Enemy enemy in pool._enemies)
                {
                    if (randomColor) enemy.SetColor(Random.ColorHSV(), colorType);
                    else enemy.SetColor(color, colorType);
                }
                return;
            }
        }
        
    }
}

[System.Serializable]
public class ShapePalette
{
    public Shape shape;

    public void ApplyPalette(Material shaderMaterial)
    {
        shape.SetColor(Player.Instance.Shape.GetColor());
        shape.SetColorType((Shape.ColorType)Player.Instance.Shape.GetColorType());
    }
}