using System.Collections.Generic;
using System.Linq;
using Common;
using UnityEngine;

public partial class MainManager
{
    private int _itemLever = 10; // =10

    private void GetResouece()
    {
        if (_images.Count > 0) return;

        var nums = Enumerable.Range(1, 36);
        foreach (var num in nums)
        {
            var image = Resources.Load<Sprite>(string.Format(Constants.ImagePath,num));
            var imageWithType = new ImageWithType(image, num);

            _images.AddRange(new[] { imageWithType, imageWithType, imageWithType, imageWithType });
        }

        //fix
        RamdomImages();
    }

    private void RamdomImages()
    {
        var listIndex = new List<int>();
        var total = _images.Count - 10;
        while (true)
        {
            Debug.Log(listIndex.Count + " listIndex.Count");
            if (listIndex.Count == _itemLever)
            {
                break;
            }
            var randomInt = Random.Range(0, total);
            if (listIndex.Contains(randomInt)
                || listIndex.Contains(randomInt + 1)
                || listIndex.Contains(randomInt - 1)) continue;
            
            listIndex.Add(randomInt);
            listIndex.Add(randomInt + 1);
        }

        List<ImageWithType> listTempSugget = new List<ImageWithType>();
        List<ImageWithType> listTempRandom = new List<ImageWithType>();

        for (var i = 0; i < _images.Count; i++)
        {
            if (listIndex.Contains(i))
            {
                listTempSugget.Add(_images[i]);
            }
            else
            {
                listTempRandom.Add(_images[i]);
            }
        }

        _images = listTempRandom
            .OrderBy(e => Random.Range(0, _width * _height))
            .ToList();
        
        listIndex = listIndex.OrderBy(e => e).ToList();
        
        for (var i = 0; i < listIndex.Count; i++)
        {
            _images.Insert(listIndex[i], listTempSugget[i]);
        }
    }
    private void GenerateGridHorizontalBot()
    {
        int count = 0;
        for (int x = 0; x < _height; x++)
        {
            for (int y = 0; y < _width; y++)
            {
                var index = x * _width + y + 1;

                var spawnedItem = Instantiate(
                    _cardItem,
                    new Vector3(y - (_width / 2f - 0.5f), x - (_height / 2f - 0.5f)),
                    Quaternion.identity,
                    parentObj);
                
                
                var isHas = !(y == 0 || y == _width - 1 || x == 0 || x == _height - 1);

                
                var imageWithType = isHas ? _images[count] : new ImageWithType();
                imageWithType.Id = index;
                spawnedItem.name = string.Format(Constants.ImageName,x,y,imageWithType.TypeImage);
                // spawnedItem.name = $"Card Item {x} {y} {imageWithType.TypeImage}";

                spawnedItem.Init(this, imageWithType);
                spawnedItem.IsHas = isHas;
                spawnedItem.TypeImage = imageWithType.TypeImage;
                spawnedItem.Id = index;

                _spawnedItemsList.Add(spawnedItem);

                if (isHas) count++;
            }
        }
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
                spawnedItem.name = string.Format(Constants.ImageName,x,y,imageWithType.TypeImage);
                // spawnedItem.name = $"Card Item {x} {y} {imageWithType.TypeImage}";

                spawnedItem.Init(this, imageWithType);
                spawnedItem.IsHas = isHas;
                spawnedItem.TypeImage = imageWithType.TypeImage;
                spawnedItem.Id = index;

                _spawnedItemsList.Add(spawnedItem);

                if (isHas) count++;
            }
        }
    }
}