using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UILine : Image
{
    float _lineWidth;

    RectTransform _tr;

    void Awake()
    {
        _tr = GetComponent<RectTransform>();
    }

    public UILine SetWidth(float width)
    {
        _lineWidth = width;
        return this;
    }

    public UILine SetPositions(Vector2 pointA, Vector2 pointB)
    {
        Vector3 differenceVector = pointB - pointA;
        var line = this.gameObject;
        _tr.sizeDelta = new Vector2(differenceVector.magnitude, _lineWidth);
        _tr.pivot = new Vector2(0, 0.5f);
        _tr.localPosition = new Vector3(pointA.x, pointA.y, transform.position.z);
        _tr.localScale = new Vector3(1f, _tr.localScale.y, 1f);
        float angle = Mathf.Atan2(differenceVector.y, differenceVector.x) * Mathf.Rad2Deg;
        line.transform.localRotation = Quaternion.Euler(0, 0, angle);
        return this;
    } 
}
