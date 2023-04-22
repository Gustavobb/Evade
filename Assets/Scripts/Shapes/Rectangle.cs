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
    }

    public override Vector4 GetProperties()
    {
        Vector4 props = new Vector4(transform.position.x / _factor, transform.position.y / _factor,
        _boxCollider.size.x / (_factor * 2)  * transform.localScale.x,
        _boxCollider.size.y / (_factor * 2) * transform.localScale.y);

        if (_accountParentScale)
        {
            props.z *= transform.parent.localScale.x;
            props.w *= transform.parent.localScale.y;
        }

        return props;
    }

    public override Vector4 GetExtra()
    {
        return new Vector4((float) _colorType, -transform.rotation.eulerAngles.z, _hasShadow ? 1 : 0, _blendBlack);
    }
}
