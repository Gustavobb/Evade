using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaletteManager : MonoBehaviour
{
    [SerializeField] private Material shaderMaterial;
    [SerializeField] List<Palette> palettes = new List<Palette>();

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
    public bool randomBackgroundColor;
    public Color backgroundColor;

    public List<EnemyPalette> enemyPalettes = new List<EnemyPalette>();

    public void ApplyPalette(Material shaderMaterial)
    {
        if (randomBackgroundColor) shaderMaterial.SetColor("_Background", Random.ColorHSV());
        else shaderMaterial.SetColor("_Background", backgroundColor);

        foreach (EnemyPalette enemyPalette in enemyPalettes)
            enemyPalette.ApplyPalette(shaderMaterial);
    }
}

[System.Serializable]
public class EnemyPalette
{
    public Enemy enemy;
    public Shape.ColorType colorType;
    public bool randomColor;
    public Color color;

    public void ApplyPalette(Material shaderMaterial)
    {
        if (randomColor) enemy.SetColor(Random.ColorHSV(), colorType);
        else enemy.SetColor(color, colorType);
    }
}