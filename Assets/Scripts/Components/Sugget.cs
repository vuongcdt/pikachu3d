using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

public partial class MainManager
{
    private void GetSuggest()
    {
        var result = new List<ItemStore>();
        for (var i = 0; i < _itemsStore.Count - _height; i++)
        {
            var entrySameRow = _itemsStore[i].TypeImage == _itemsStore[i + 1].TypeImage
                               && _itemsStore[i].IsHas
                               && _itemsStore[i + 1].IsHas;
            var entrySameCol = _itemsStore[i].TypeImage == _itemsStore[i + _height].TypeImage
                               && _itemsStore[i].IsHas
                               && _itemsStore[i + _height].IsHas;

            if (entrySameRow)
            {
                result.AddRange(new[] { _itemsStore[i], _itemsStore[i + 1] });
                break;
            }

            if (entrySameCol)
            {
                result.AddRange(new[] { _itemsStore[i], _itemsStore[i + _height] });
                break;
            }
        }

        var canCompareList = new List<ItemStore>();
        var canNotCompareList = new List<ItemStore>();

        for (var i = _height; i < _itemsStore.Count - _height; i++)
        {
            if (i % _height == 0 || i % _height == _height || !_itemsStore[i].IsHas) continue;
            if (_itemsStore[i + 1].IsHas
                && _itemsStore[i - 1].IsHas
                && _itemsStore[i + _height].IsHas
                && _itemsStore[i - _height].IsHas)
                canNotCompareList.Add(_itemsStore[i]);
            else
                canCompareList.Add(_itemsStore[i]);
        }

        var res = canCompareList
            .GroupBy(e => e.TypeImage)
            .Select(e => new
            {
                Key = e.Key,
                Total = e.Count(),
                ItemsList = e.ToList()
            })
            .Where(e => e.Total > 1);

        foreach (var re in res)
        {
            Debug.Log(re.Key + "__" + re.Total);
        }

        Debug.Log(res.Count() + "??????????");
        if (result.Count > 0)
        {
            RenderLineSuggest(result);
            return;
        }
    }

    private void RenderLineSuggest(List<ItemStore> itemStores)
    {
        foreach (var itemStore in itemStores)
        {
            RenderLineSuggest(itemStore);
        }
    }

    private void RenderLineSuggest(ItemStore itemStore)
    {
        var x = itemStore.X;
        var y = itemStore.Y;

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