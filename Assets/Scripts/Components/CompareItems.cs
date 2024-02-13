using System;
using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

public partial class MainManager
{
    private bool CompareItems(CardItem firstItem, CardItem lastItem)
    {
        var itemsNoValue = _spawnedItemsList
            .Where(e => !e.IsHas || e.Id == lastItem.Id || e.Id == firstItem.Id);

        var checkByVertical = CheckNoValueByAxis(Axis.Vertical, itemsNoValue, firstItem, lastItem);
        if (checkByVertical)
        {
            if (_isPassItem) Invoke(nameof(SetHideWorkingItem), 0.2f);
            return true;
        }

        var checkByHorizontal = CheckNoValueByAxis(Axis.Horizontal, itemsNoValue, firstItem, lastItem);
        if (checkByHorizontal)
        {
            if (_isPassItem) Invoke(nameof(SetHideWorkingItem), 0.2f);
            return true;
        }

        if (_isPassItem) SetDefaultWorkingItem();
        return false;
    }

    private bool CheckNoValueByAxis(
        Axis axis,
        IEnumerable<CardItem> itemsNoValue,
        CardItem firstItem,
        CardItem lastItem)
    {
        var minX = firstItem.FlipAxis(axis).x;
        var yOfMinX = firstItem.FlipAxis(axis).y;
        var maxX = lastItem.FlipAxis(axis).x;
        var yOfMaxX = lastItem.FlipAxis(axis).y;

        if (minX > maxX)
        {
            (minX, maxX) = (maxX, minX);
            (yOfMinX, yOfMaxX) = (yOfMaxX, yOfMinX);
        }

        itemsNoValue = itemsNoValue
            .Where(e => e.FlipAxis(axis).x >= minX && e.FlipAxis(axis).x <= maxX);

        var distance = (int)(maxX - minX);

        var checkVertical = itemsNoValue
            .GroupBy(e => e.FlipAxis(axis).y)
            .Where(e => e.Count() == distance + 1)
            .Select(e => e.Key)
            .OrderBy(e => Math.Abs((yOfMaxX + yOfMinX) / 2 - e));

        var isPass = false;

        foreach (var tempY in checkVertical)
        {
            var entryMinX = CheckNoValueByAxis(axis, itemsNoValue, tempY, yOfMinX, minX);
            var entryMaxX = CheckNoValueByAxis(axis, itemsNoValue, tempY, yOfMaxX, maxX);
            if (entryMinX && entryMaxX)
            {
                if (_isPassItem)
                {
                    RemoveLine();
                    RenderLinePass(tempY, axis);
                }

                isPass = true;
                break;
            }
            // Debug.Log(isPass + "isPass");
        }

        return isPass;
    }

    private bool CheckNoValueByAxis(Axis axis, IEnumerable<CardItem> itemsNoValue, float minY, float maxY, float x)
    {
        if (minY > maxY) (minY, maxY) = (maxY, minY);
        
        var distance = (int)(maxY - minY) + 1;

        var totalCard = itemsNoValue
            .Count(e => e.FlipAxis(axis).x == x
                        && e.FlipAxis(axis).y >= minY
                        && e.FlipAxis(axis).y <= maxY);
        return totalCard == distance;
    }

    private void RenderLinePass(float tempY, Axis axis)
    {
        var xOfPoint2 = _firstItem.FlipAxis(axis).x;
        var xOfPoint3 = _lastItem.FlipAxis(axis).x;
        var yOfPoint2 = tempY;
        var yOfPoint3 = tempY;
        if (axis == Axis.Horizontal)
        {
            xOfPoint2 = tempY;
            xOfPoint3 = tempY;
            yOfPoint2 = _firstItem.FlipAxis(axis).x;
            yOfPoint3 = _lastItem.FlipAxis(axis).x;
        }

        Vector3[] points = new[]
        {
            _firstItem.transform.position,
            new Vector3(xOfPoint2, yOfPoint2, ZLine),
            new Vector3(xOfPoint3, yOfPoint3, ZLine),
            _lastItem.transform.position
        };

        _lineRenderer.positionCount = 4;
        _lineRenderer.SetPositions(points);

        _lines.Add(Instantiate(
            _lineRenderer,
            points[0],
            Quaternion.identity,
            parentObj));
        Invoke(nameof(RemoveLine), 0.2f);
    }

    private void SetHideWorkingItem()
    {
        _spawnedItemsList.ForEach(e =>
        {
            if (e.Id == _firstItem.Id || e.Id == _lastItem.Id)
                e.IsHas = false;
        });
        _firstItem.SetShow(false);
        _lastItem.SetShow(false);

        SetDefaultWorkingItem();
        GetSuggest();
    }

    private void RemoveLine()
    {
        _lines.ForEach(e => e.positionCount = 0);
    }
}