using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Common;
using Unity.VisualScripting;
using Random = UnityEngine.Random;

public class MainManager : MonoBehaviour
{
    [SerializeField] private int _width = 18, _height = 11;

    [SerializeField] private Transform parentObj;
    [SerializeField] private CardItem _cardItem;
    [SerializeField] private LineRenderer _lineRenderer;

    private CardItem _firstItem, _lastItem;

    private List<ImageWithType> _images = new List<ImageWithType>();
    private List<ItemStore> _itemsStore = new List<ItemStore>();
    private List<ItemStore> _drawLine = new List<ItemStore>();
    private List<CardItem> spawnedItemsList = new List<CardItem>();
    private LineRenderer line;

    private void Awake()
    {
        GetResouece();
        GenerateGrid();
        Debug.Log(spawnedItemsList.Count);
        // spawnedItemsList.ForEach(e =>
        // {
        //     // e._spriteRenderer.sprite.border.Set(2,2,2,2);
        //     e._spriteRenderer.color = Color.gray;
        // });
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
        cardItem._spriteRenderer.color = Color.grey;

        cardItem.IfIsNull(() => { SetDefaultWorkingItem(); });

        if (_firstItem == null)
        {
            Debug.Log("set first item!" + cardItem.ToString());
            _firstItem = cardItem;
            return;
        }

        if (_firstItem.Id == cardItem.Id)
        {
            SetDefaultWorkingItem();
            return;
        }

        Debug.Log("Set last item!" + cardItem.ToString());
        _lastItem = cardItem;
        if (_firstItem.TypeImage != _lastItem.TypeImage)
        {
            // Debug.Log("Not same!");
            SetDefaultWorkingItem();
            return;
        }

        CompareItems();
    }

    private void CompareItems()
    {
        var itemsNoValue = _itemsStore
            .Where(e => !e.IsHas || e.Id == _lastItem.Id || e.Id == _firstItem.Id);

        var checkByVertical = CheckNoValueByAxis(Axis.Vertical, itemsNoValue);
        if (checkByVertical)
        {
            Invoke("SetHideWorkingItem", 0.2f);
            return;
        }

        var checkByHorizontal = CheckNoValueByAxis(Axis.Horizontal, itemsNoValue);

        // Debug.Log(checkByVertical + " checkByVertical");
        // Debug.Log(checkByHorizontal + " checkByHorizontal");
        if (checkByHorizontal)
        {
            Invoke("SetHideWorkingItem", 0.2f);
            // SetHideWorkingItem();
            return;
        }
        SetDefaultWorkingItem();
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

        var isPass = false;

        foreach (var tempY in checkVertical)
        {
            var entryMinX = CheckNoValueByAxis(axis, itemStores, tempY, YOfMinX, minX);
            var entryMaxX = CheckNoValueByAxis(axis, itemStores, tempY, YOfMaxX, maxX);
            if (entryMinX && entryMaxX)
            {
                RenderLine(tempY, axis);

                // Debug.Log(tempY + "////////////");
                // var drawLine = spawnedItemsList.Where(e =>
                //     e.FlipAxis(axis).y == tempY 
                //     && e.FlipAxis(axis).x >= minX 
                //     && e.FlipAxis(axis).x <= maxX);
                //
                // Debug.Log(drawLine.Count()+"|||||||");
                // foreach (var itemStore in drawLine)
                // {
                //     itemStore._spriteRenderer.sprite = _images[0].Sprite;
                //     itemStore._spriteRenderer.color = Color.red;
                //     itemStore._spriteRenderer.size = new Vector2(1,1);
                // }

                isPass = true;
                break;
            }

            Debug.Log(isPass + "isPass");
        }

        return isPass;
    }

    private void RenderLine(float tempY, Axis axis)
    {
        var xOfPoint2 = _firstItem.FlipAxis(axis).x;
        var xOfPoint3 = _lastItem.FlipAxis(axis).x;
        var yOfPoint2 = tempY;
        var yOfPoint3 = tempY;
        if (axis == Axis.Horizontal)
        {
            xOfPoint2 = xOfPoint3 = tempY;
            yOfPoint2 = _firstItem.FlipAxis(axis).x;
            yOfPoint3 = _lastItem.FlipAxis(axis).x;
        }

        Vector3[] points = new[]
        {
            _firstItem.transform.position,
            new Vector3(xOfPoint2, yOfPoint2, -0.1f),
            new Vector3(xOfPoint3, yOfPoint3, -0.1f),
            _lastItem.transform.position
        };

        _lineRenderer.positionCount = 4;
        for (var i = 0; i < points.Length; i++)
        {
            _lineRenderer.SetPosition(i, points[i]);
        }

        line = Instantiate(
            _lineRenderer,
            points[0],
            Quaternion.identity,
            parentObj);
        Invoke("RemoveLine", 0.2f);
    }

    private void RemoveLine()
    {
        line.positionCount = 0;
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
        _firstItem._spriteRenderer.color = Color.white;
        _lastItem._spriteRenderer.color = Color.white;
        _firstItem = null;
        _lastItem = null;
        // spawnedItemsList.ForEach(e => e._spriteRenderer.color = Color.white);

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

        SetDefaultWorkingItem();
    }
}