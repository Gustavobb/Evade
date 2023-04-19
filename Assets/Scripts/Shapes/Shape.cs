using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] protected float _factor = 10f;
    [SerializeField] protected Color _color = Color.white;

    protected enum ColorType { Custom, Rainbow }
    [SerializeField] protected ColorType _colorType = ColorType.Custom;

    public virtual Vector4 GetCenter()
    {
        return  new Vector4(transform.position.x / _factor, transform.position.y / _factor, 0, 0);
    }

    public virtual Vector4 GetData()
    {
        return Vector4.zero;
    }

    public virtual Color GetColor()
    {
        return _color;
    }
}