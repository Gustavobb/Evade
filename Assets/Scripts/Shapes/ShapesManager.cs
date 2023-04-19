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
    
    private Vector4[] _shapesCenter = new Vector4[100];
    private Vector4[] _shapesData = new Vector4[100];
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
        Vector4 shapeCenter;
        Vector4 shapeData;
        Color shapeColor;
        
        for (int i = 0; i < shapes.Count; i++)
        {
            if (!shapes[i].gameObject.activeSelf) continue;
            shapeCenter = shapes[i].GetCenter();
            _shapesCenter[i] = shapeCenter;

            shapeData = shapes[i].GetData();
            _shapesData[i] = shapeData;

            shapeColor = shapes[i].GetColor();
            _shapesColors[i] = shapeColor;
        }

        _shapesMaterial.SetInt($"_{name}Count", shapes.Count);
        _shapesMaterial.SetVectorArray($"_{name}Center", _shapesCenter);
        _shapesMaterial.SetVectorArray($"_{name}Data", _shapesData);
        _shapesMaterial.SetColorArray($"_{name}Color", _shapesColors);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(source, destination, _shapesMaterial);
        Graphics.Blit(destination, _previousFrameTex);
    }
}
