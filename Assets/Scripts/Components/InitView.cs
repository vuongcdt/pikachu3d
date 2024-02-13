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
            var imageWithId = new ImageWithType(image, num);

            _images.Add(imageWithId);
            _images.Add(imageWithId);
            _images.Add(imageWithId);
            _images.Add(imageWithId);
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

                var imageWithId = isHas ? _images[count] : new ImageWithType();
                imageWithId.Id = index;
                spawnedItem.name = $"Card Item {x} {y} {imageWithId.TypeImage}";

                spawnedItem.Init(this, imageWithId);
                spawnedItem.IsHas = isHas;
                spawnedItem.TypeImage = imageWithId.TypeImage;
                spawnedItem.Id = index;
                
                var position = spawnedItem.transform.position;
                
                // _itemsStore.Add(new ItemStore(
                //     position.x,
                //     position.y,
                //     index,
                //     imageWithId.TypeImage,
                //     isHas
                // ));

                _spawnedItemsList.Add(spawnedItem);

                if (isHas) count++;
            }
        }
    }
}