using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shape : MonoBehaviour
{
    [SerializeField] protected float _factor = 10f;
    [SerializeField] protected Color _color = Color.white;

    protected enum ColorType { Custom, Rainbow }
    [SerializeField] protected ColorType _colorType = ColorType.Custom;
    [SerializeField] protected float _blendBlack = 1f;
    [SerializeField] protected bool _hasShadow = false;
    // [SerializeField] protected Color _shadowColor = Color.black;

    [SerializeField] protected bool _hasOutline = false;
    // [SerializeField] protected Color _outlineColor = Color.black;

    [SerializeField] protected bool _accountParentScale = false;

    public virtual Vector4 GetProperties()
    {
        return new Vector4(transform.position.x / _factor, transform.position.y / _factor, 0, 0);
    }

    public virtual Vector4 GetExtra()
    {
        return Vector4.zero;
    }

    public virtual Color GetColor()
    {
        return _color;
    }

    public virtual float GetColorType()
    {
        return (float) _colorType;
    }

    public virtual void SetBlendBlack(float blendBlack)
    {
        _blendBlack = blendBlack;
    }

    public virtual void LerpBlendBlack(float start, float end, float duration)
    {
        StartCoroutine(LerpBlendBlackCoroutine(start, end, duration));
    }

    IEnumerator LerpBlendBlackCoroutine(float start, float end, float duration)
    {
        float time = 0;
        while (time < duration)
        {
            _blendBlack = Mathf.Lerp(start, end, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
    }
}