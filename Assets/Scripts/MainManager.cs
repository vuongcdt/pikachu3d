using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common;
using Random = UnityEngine.Random;

public class MainManager : MonoBehaviour
{
    [SerializeField] private int _width = 18, _height = 11;

    [SerializeField] private Transform parentObj;
    [SerializeField] private CardItem _cardItem;

    private List<ImageWithType> _images = new List<ImageWithType>();

    private List<ItemStore> _itemsStore = new List<ItemStore>();
    private IEnumerable<int> _itemsNoValueByAxis;

    private CardItem _firstItem, _lastItem;
    private List<CardItem> spawnedItemsList = new List<CardItem>();

    private void Awake()
    {
        GetResouece();
        GenerateGrid();
    }

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

                spawnedItem.name = $"Card Item {x} {y}";

                var isHas = !(y == 0 || y == _height - 1 || x == 0 || x == _width - 1);

                var imageWithId = isHas ? _images[count] : new ImageWithType();
                imageWithId.Id = index;

                spawnedItem.Init(this, imageWithId);
                _itemsStore.Add(new ItemStore(
                    spawnedItem.transform.position.x,
                    spawnedItem.transform.position.y,
                    index,
                    imageWithId.TypeImage,
                    isHas
                ));
                
                spawnedItemsList.Add(spawnedItem);

                if (isHas) count++;
            }
        }
    }
    
    public void SetItem(CardItem cardItem)
    {
        cardItem.IfIsNull(() => { SetDefaultWorkingItem(); });

        if (_firstItem == null)
        {
            Debug.Log("set first item!" + cardItem.ToString());
            _firstItem = cardItem;
            return;
        }

        if(_firstItem.Id == cardItem.Id)
        {
            SetDefaultWorkingItem();
            return;
        }
        
        Debug.Log("Set last item!" + cardItem.ToString());
        _lastItem = cardItem;
        if (_firstItem.TypeImage != _lastItem.TypeImage)
        {
            Debug.Log("Not same!");
            SetDefaultWorkingItem();
            return;
        }

        CompareItems();

        SetDefaultWorkingItem();
    }

    private void CompareItems()
    {
        var itemsNoValue = _itemsStore
            .Where(e => !e.IsHas || e.Id == _lastItem.Id || e.Id == _firstItem.Id);

        var checkByVertical = CheckNoValueByAxis(Axis.Vertical, itemsNoValue);
        var checkByHorizontal = CheckNoValueByAxis(Axis.Horizontal, itemsNoValue);

        Debug.Log(checkByVertical + " checkByVertical");
        Debug.Log(checkByHorizontal + " checkByHorizontal");
        if(checkByVertical || checkByHorizontal)
        {
            SetHideWorkingItem();
        }
    }

    private bool CheckNoValueByAxis(Axis axis, IEnumerable<ItemStore> itemStores)
    {
        var minX = _firstItem.FlipAxis(axis).x;
        var YOfMinX = _firstItem.FlipAxis(axis).y;
        var maxX = _lastItem.FlipAxis(axis).x;
        var YOfMaxX = _lastItem.FlipAxis(axis).y;

        if (minX > maxX)
        {
            (minX, maxX) = (maxX, minX);
            (YOfMinX, YOfMaxX) = (YOfMaxX, YOfMinX);
        }

        itemStores = itemStores
            .Where(e => e.FlipAxis(axis).x >= minX && e.FlipAxis(axis).x <= maxX);
        // .Where(e => e.X >= minX && e.X <= maxX || e.Y >= minY && e.Y <= maxY);

        var distance = (int)(maxX - minX);

        var checkVertical = itemStores
            .GroupBy(e => e.FlipAxis(axis).y)
            .Where(e => e.Count() == distance + 1)
            .Select(e => e.Key);

        var result = false;

        foreach (var tempY in checkVertical)
        {
            var entryMinX = CheckNoValueByAxis(axis, itemStores, tempY, YOfMinX, minX);
            var entryMaxX = CheckNoValueByAxis(axis, itemStores, tempY, YOfMaxX, maxX);
            if (entryMinX && entryMaxX)
            {
                result = true;
                break;
            }
        }

        return result;
    }

    private bool CheckNoValueByAxis(Axis axis, IEnumerable<ItemStore> itemStores, float minY, float maxY, float x)
    {
        if (minY > maxY) (minY, maxY) = (maxY, minY);
        var distance = (int)(maxY - minY) + 1;

        var totalCard = itemStores
            .Count(e => e.FlipAxis(axis).x == x
                        && e.FlipAxis(axis).y >= minY
                        && e.FlipAxis(axis).y <= maxY);
        return totalCard == distance;
    }

    private void SetDefaultWorkingItem()
    {
        _firstItem = null;
        _lastItem = null;
    }

    private void SetHideWorkingItem()
    {
        _itemsStore.ForEach(e =>
        {
            if (e.Id == _firstItem.Id || e.Id == _lastItem.Id) 
                e.IsHas = false;
        });
        _firstItem.SetShow(false);
        _lastItem.SetShow(false);
 
    }
}