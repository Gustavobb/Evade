using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Circle : Shape
{
    private CircleCollider2D _circleCollider;

    private void Awake()
    {
        _circleCollider = GetComponent<CircleCollider2D>();
        ShapesManager.Instance.AddShape(this);
    }

    public override Vector4 GetData()
    {
        return new Vector4(_circleCollider.radius / _factor * transform.localScale.x, 0, 0, (int) _colorType);
    }

    private void OnDestory()
    {
        ShapesManager.Instance.RemoveShape(this);
    }
}