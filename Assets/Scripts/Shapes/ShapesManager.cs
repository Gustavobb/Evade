using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesManager : MonoBehaviour
{
    [SerializeField] private Material _shapesMaterial;
    [SerializeField] private RenderTexture _previousFrameTex;

    // Circles
    [SerializeField] private List<Circle> _circles = new List<Circle>();

    // Rectangles
    [SerializeField] private List<Rectangle> _rectangles = new List<Rectangle>();
    
    private Vector4[] _shapesProperties = new Vector4[50];
    private Vector4[] _shapesExtra = new Vector4[50];
    private Color[] _shapesColors = new Color[50];

    private static ShapesManager _instance;
    public static ShapesManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<ShapesManager>();
            return _instance;
        }
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        CreateRenderTexture();
    }

    private void Update()
    {
        UpdateShapes<Circle>(_circles, "Circles");
        UpdateShapes<Rectangle>(_rectangles, "Rectangles");
    }

    public void AddShape<T>(T shape) where T : Shape
    {
        switch (shape)
        {
            case Circle circle:
                _circles.Add(circle);
                break;
            case Rectangle rectangle:
                _rectangles.Add(rectangle);
                break;
        }
    }

    public void RemoveShape<T>(T shape) where T : Shape
    {
        switch (shape)
        {
            case Circle circle:
                _circles.Remove(circle);
                break;
            case Rectangle rectangle:
                _rectangles.Remove(rectangle);
                break;
        }
    }

    private void CreateRenderTexture()
    {
        if (_previousFrameTex != null)
            _previousFrameTex.Release();

        _previousFrameTex = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        _previousFrameTex.Create();
        _shapesMaterial.SetTexture("_PrevFrame", _previousFrameTex);
    }

    private void UpdateShapes<T>(in List<T> shapes, string name) where T : Shape
    {
        Vector4 shapeProperties;
        Vector4 shapeExtra;
        Color shapeColor;
        int count = 0;

        for (int i = 0; i < shapes.Count; i++)
        {
            if (!shapes[i].gameObject.activeInHierarchy) continue;
            shapeProperties = shapes[i].GetProperties();
            _shapesProperties[count] = shapeProperties;

            shapeExtra = shapes[i].GetExtra();
            _shapesExtra[count] = shapeExtra;

            shapeColor = shapes[i].GetColor();
            _shapesColors[count] = shapeColor;

            count++;
        }

        _shapesMaterial.SetInt($"_{name}Count", count);
        _shapesMaterial.SetVectorArray($"_{name}Properties", _shapesProperties);
        _shapesMaterial.SetVectorArray($"_{name}Extra", _shapesExtra);
        _shapesMaterial.SetColorArray($"_{name}Color", _shapesColors);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _shapesMaterial);
        Graphics.Blit(destination, _previousFrameTex);
    }
}
