using System.Collections.Generic;
using UnityEngine;
using Common;

public partial class MainManager : MonoBehaviour
{
    [SerializeField] private int _width = 18, _height = 11;

    [SerializeField] private Transform parentObj;
    [SerializeField] private CardItem _cardItem;
    [SerializeField] private LineRenderer _lineRenderer;

    private CardItem _firstItem, _lastItem;

    private List<ImageWithType> _images = new List<ImageWithType>();
    // private readonly List<ItemStore> _itemsStore = new List<ItemStore>();
    private readonly List<CardItem> _spawnedItemsList = new List<CardItem>();
    private List<CardItem> _suggetItems = new List<CardItem>();
    private readonly List<LineRenderer> _lines = new List<LineRenderer>();

    private const float ZLine = -0.01f;

    private void Awake()
    {
        GetResouece();
        GenerateGrid();
        // GetSuggest();
    }

    public void SetItem(CardItem cardItem)
    {
        cardItem._spriteRenderer.color = Color.grey;

        cardItem.IfIsNull(SetDefaultWorkingItem);

        if (_firstItem == null)
        {
            Debug.Log("Set first item!" + cardItem.ToString());
            _firstItem = cardItem;
            return;
        }

        Debug.Log("Set last item!" + cardItem.ToString());
        _lastItem = cardItem;

        if (_firstItem.Id == cardItem.Id)
        {
            SetDefaultWorkingItem();
            return;
        }

        if (_firstItem.TypeImage != _lastItem.TypeImage)
        {
            Debug.Log("Not same!");
            SetDefaultWorkingItem();
            return;
        }

        CompareItems(_firstItem, _lastItem);
    }

    private void SetDefaultWorkingItem()
    {
        Invoke(nameof(ClearItem),0.2f);
    }

    private void ClearItem()
    {
        _firstItem._spriteRenderer.color = Color.white;
        _lastItem._spriteRenderer.color = Color.white;
        _firstItem = null;
        _lastItem = null;
    }
}