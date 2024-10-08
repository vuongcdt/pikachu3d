﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainManager
{
    private void GetSuggest(bool isActive = true)
    {
        _suggetItems = new List<CardItem>();

        SuggetNotTogether();

        if (_suggetItems.Count > 0)
        {
            if(isActive)
            {
                SuggestLine();
            }
        }
        else
        {
            RerenderGrid();
        }
    }

    private void SuggetNotTogether()
    {
        if (_suggetItems.Count > 0) return;

        _isPassItem = false;
        var canCompareList = new List<CardItem>();
        var canNotCompareList = new List<CardItem>();

        for (var i = _height; i < _spawnedItemsList.Count - _height; i++)
        {
            if (i % _height == 0 || i % _height == _height || !_spawnedItemsList[i].IsHas) continue;
            
            if (_spawnedItemsList[i + 1].IsHas
                && _spawnedItemsList[i - 1].IsHas
                && _spawnedItemsList[i + _height].IsHas
                && _spawnedItemsList[i - _height].IsHas)
            {
                canNotCompareList.Add(_spawnedItemsList[i]);
            }
            else
            {
                canCompareList.Add(_spawnedItemsList[i]);
            }
        }

        var compareList = canCompareList
            .GroupBy(e => e.TypeImage)
            .Select(e => new
            {
                Key = e.Key,
                Total = e.Count(),
                ItemsList = e.ToList()
            })
            .Where(e => e.Total > 1);

        foreach (var compare in compareList)
        {
            var cardItemsList = compare.ItemsList;

            for (var i = 0; i < cardItemsList.Count; i++)
            {
                for (var j = i + 1; j < cardItemsList.Count; j++)
                {
                    var entry = CompareItems(cardItemsList[i], cardItemsList[j]);
                    if (entry)
                    {
                        _suggetItems.AddRange(new[] { cardItemsList[i], cardItemsList[j] });
                        break;
                    }
                }

                if (_suggetItems.Count > 0) break;
            }

            if (_suggetItems.Count > 0) break;
        }
    }

    private void SuggestLine()
    {

        _suggetItems[0]._spriteRenderer.color = Color.grey;
        InvokeRepeating(nameof(ToggleColorCard), 0, 0.2f);
        Invoke(nameof(ClearInvoke), 2f);
    }

    private void ClearInvoke()
    {
        CancelInvoke();
        foreach (var cardItem in _suggetItems)
        {
            cardItem._spriteRenderer.color = Color.white;
        }
    }

    private void ToggleColorCard()
    {
        foreach (var cardItem in _suggetItems)
        {
            cardItem._spriteRenderer.color = cardItem._spriteRenderer.color == Color.grey
                ? Color.white
                : Color.gray;
        }
    }

    private void RenderLineSuggest(CardItem itemStore)
    {
        var position = itemStore.transform.position;
        var x = position.x;
        var y = position.y;

        var ponitStart = new Vector3(x - 0.5f, y + 0.5f, ZLine);

        Vector3[] points = new[]
        {
            ponitStart,
            ponitStart + Vector3.right,
            ponitStart + Vector3.down + Vector3.right,
            ponitStart + Vector3.down,
            ponitStart,
        };

        _lineRenderer.positionCount = 5;
        _lineRenderer.SetPositions(points);

        _lines.Add(Instantiate(
            _lineRenderer,
            ponitStart,
            Quaternion.identity,
            parentObj));
    }
}