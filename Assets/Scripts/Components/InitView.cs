using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

public partial class MainManager
{
    private void GetResouece()
    {
        if (_images.Count > 0) return;

        var nums = Enumerable.Range(1, 36);
        foreach (var num in nums)
        {
            var image = Resources.Load<Sprite>($"pieces{num}");
            var imageWithType = new ImageWithType(image, num);

            _images.Add(imageWithType);
            _images.Add(imageWithType);
            _images.Add(imageWithType);
            _images.Add(imageWithType);
        }

        //fix
        RamdomImages();
    }

    private void RamdomImages()
    {
        _images = _images
            .OrderBy(e => Random.Range(0, _width * _height))
            .ToList();
    }

    private void GenerateGrid()
    {
        int count = 0;
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var index = x * _height + y + 1;

                var spawnedItem = Instantiate(
                    _cardItem,
                    new Vector3(x - (_width / 2f - 0.5f), y - (_height / 2f - 0.5f)),
                    Quaternion.identity,
                    parentObj);

                var isHas = !(y == 0 || y == _height - 1 || x == 0 || x == _width - 1);

                var imageWithType = isHas ? _images[count] : new ImageWithType();
                imageWithType.Id = index;
                spawnedItem.name = $"Card Item {x} {y} {imageWithType.TypeImage}";

                spawnedItem.Init(this, imageWithType);
                spawnedItem.IsHas = isHas;
                spawnedItem.TypeImage = imageWithType.TypeImage;
                spawnedItem.Id = index;

                _spawnedItemsList.Add(spawnedItem);

                if (isHas) count++;
            }
        }

        // CheckTypeItem();
    }

    private void CheckTypeItem(List<CardItem> cardItems)
    {
        var checkType = cardItems
            .GroupBy(e => e.TypeImage)
            .Select(e => new
            {
                key = e.Key,
                total = e.Count()
            })
            .OrderBy(e => e.key);
        string inLog = "";
        foreach (var i in checkType)
        {
            inLog += i.key + "-" + i.total + "; ";
        }

        Debug.Log(inLog);
    }
}