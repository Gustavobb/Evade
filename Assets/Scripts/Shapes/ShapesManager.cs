using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapesManager : MonoBehaviour
{
    [SerializeField] private Material _shapesMaterial;
    [SerializeField] private RenderTexture _previousFrameTex;

    // Circles
    [SerializeField] private HashSet<Circle> _circles = new HashSet<Circle>();

    // Rectangles
    [SerializeField] private HashSet<Rectangle> _rectangles = new HashSet<Rectangle>();
    
    private Vector4[] _shapesProperties = new Vector4[100];
    private Vector4[] _shapesExtra = new Vector4[100];
    private Color[] _shapesColors = new Color[100];

    private static ShapesManager _instance;
    public static ShapesManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<ShapesManager>();
            return _instance;
        }
    }

    public Material ShapesMaterial => _shapesMaterial;

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
                if (_circles.Contains(circle)) break;
                _circles.Add(circle);
                break;
            case Rectangle rectangle:
                if (_rectangles.Contains(rectangle)) break;
                _rectangles.Add(rectangle);
                break;
        }
    }

    public void RemoveShape<T>(T shape) where T : Shape
    {
        switch (shape)
        {
            case Circle circle:
                if (!_circles.Contains(circle)) break;
                _circles.Remove(circle);
                break;
            case Rectangle rectangle:
                if (!_rectangles.Contains(rectangle)) break;
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

    private void UpdateShapes<T>(in HashSet<T> shapes, string name) where T : Shape
    {
        Vector4 shapeProperties;
        Vector4 shapeExtra;
        Color shapeColor;
        int count = 0;

        foreach (T shape in shapes)
        {
            if (count >= shapes.Count)
            {
                Debug.LogWarning($"Too many {name} to render");
                break;
            }
            if (!shape.gameObject.activeInHierarchy) continue;
            shapeProperties = shape.GetProperties();
            _shapesProperties[count] = shapeProperties;

            shapeExtra = shape.GetExtra();
            _shapesExtra[count] = shapeExtra;

            shapeColor = shape.GetColor();
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
