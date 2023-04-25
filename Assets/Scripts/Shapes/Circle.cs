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
    }

    public override Vector4 GetProperties()
    {
        Vector4 props = new Vector4(transform.position.x / _factor, transform.position.y / _factor, 
        _circleCollider.radius / _factor * transform.localScale.x, 0);

        if (_accountParentScale)
        {
            props.z *= transform.parent.localScale.x;
        }

        return props;
    }

    public override Vector4 GetExtra()
    {
        return new Vector4((float) _colorType + Mathf.Clamp(_blendBlack, .001f, .99f), _hasShadow ? 1 : 0, 0, _sortOrder);
    }
}