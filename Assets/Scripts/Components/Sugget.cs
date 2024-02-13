using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

public partial class MainManager
{
    private void GetSuggest()
    {
        SuggetTogether();
        SuggetNotTogether();
    }

    private void SuggetTogether()
    {
        var result = new List<ItemStore>();
        // for (var i = 0; i < _itemsStore.Count - _height; i++)
        // {
        //     var entryTogetherRow = _itemsStore[i].TypeImage == _itemsStore[i + 1].TypeImage
        //                            && _itemsStore[i].IsHas
        //                            && _itemsStore[i + 1].IsHas;
        //     var entryTogetherCol = _itemsStore[i].TypeImage == _itemsStore[i + _height].TypeImage
        //                            && _itemsStore[i].IsHas
        //                            && _itemsStore[i + _height].IsHas;
        //
        //     if (entryTogetherRow)
        //     {
        //         result.AddRange(new[] { _itemsStore[i], _itemsStore[i + 1] });
        //         break;
        //     }
        //
        //     if (entryTogetherCol)
        //     {
        //         result.AddRange(new[] { _itemsStore[i], _itemsStore[i + _height] });
        //         break;
        //     }
        // }

        if (result.Count > 0)
            RenderLineSuggest(result);
    }

    private void SuggetNotTogether()
    {
        _suggetItems = new List<CardItem>();
        var canCompareList = new List<CardItem>();
        var canNotCompareList = new List<CardItem>();
        // Debug.Log(_spawnedItemsList.Count+"XXXXXXXXXXXXXXXXXXXXX");

        for (var i = _height; i < _spawnedItemsList.Count - _height; i++)
        {
            if (i % _height == 0 || i % _height == _height || !_spawnedItemsList[i].IsHas) continue;
            if (_spawnedItemsList[i + 1].IsHas
                && _spawnedItemsList[i - 1].IsHas
                && _spawnedItemsList[i + _height].IsHas
                && _spawnedItemsList[i - _height].IsHas)
                canNotCompareList.Add(_spawnedItemsList[i]);
            else
                canCompareList.Add(_spawnedItemsList[i]);
        }

        // Debug.Log(canCompareList.Count+"______________________");
        // Debug.Log(canNotCompareList.Count+"************************");

        var compareList = canCompareList
            .GroupBy(e => e.TypeImage)
            .Select(e => new
            {
                Key = e.Key,
                Total = e.Count(),
                ItemsList = e.ToList()
            })
            .Where(e => e.Total > 1);
        // Debug.Log(compareList.Count()+"+++++++++++++++++");

        foreach (var compare in compareList)
        {
            Debug.Log(compare.Key + "__" + compare.Total);
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

        // Debug.Log(_suggetItems.Count() + "??????????");
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