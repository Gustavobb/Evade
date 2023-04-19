using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Rectangle : Shape
{
    private BoxCollider2D _boxCollider;

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider2D>();
        ShapesManager.Instance.AddShape(this);
    }

    public override Vector4 GetData()
    {
        return new Vector4(_boxCollider.size.x / (_factor * 2)  * transform.localScale.x, 
        _boxCollider.size.y / (_factor * 2) * transform.localScale.y,
        -transform.rotation.eulerAngles.z, (int) _colorType);
    }

    private void OnDestory()
    {
        print("Rectangle Destroyed");
        ShapesManager.Instance.RemoveShape(this);
    }
}
