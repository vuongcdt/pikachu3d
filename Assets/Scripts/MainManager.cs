using System;
using System.Collections.Generic;
using UnityEngine;
using Common;
using UnityEngine.UI;

public partial class MainManager : MonoBehaviour
{
    [SerializeField] private int _width = 18, _height = 11;

    [SerializeField] private Transform parentObj;
    [SerializeField] private CardItem _cardItem;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] public Button _suggetBtn;
    [SerializeField] public Text _scoreText;

    private CardItem _firstItem, _lastItem;
    private bool _isPassItem = false;
    private int _score;

    private List<ImageWithType> _images = new List<ImageWithType>();
    private readonly List<CardItem> _spawnedItemsList = new List<CardItem>();
    private List<CardItem> _suggetItems = new List<CardItem>();
    private readonly List<LineRenderer> _lines = new List<LineRenderer>();

    private const float ZLine = -0.01f;

    private void Awake()
    {
        GetResouece();
        GenerateGrid();
    }

    private void Start()
    {
        _scoreText.text = "Điểm: 0";
        _suggetBtn.onClick.AddListener(() =>
        {
            GetSuggest();
        });
    }

    private void Update()
    {
        var entry = Input.GetKey(KeyCode.Space);
        if(entry) GetSuggest();
    }

    public void SetItem(CardItem cardItem)
    {
        ClearInvoke();
        cardItem._spriteRenderer.color = Color.grey;

        cardItem.IfIsNull(SetDefaultWorkingItem);

        if (_firstItem == null)
        {
            // Debug.Log("Set first item!" + cardItem.ToString());
            _firstItem = cardItem;
            return;
        }

        // Debug.Log("Set last item!" + cardItem.ToString());
        _lastItem = cardItem;

        if (_firstItem.Id == cardItem.Id)
        {
            SetDefaultWorkingItem();
            return;
        }

        if (_firstItem.TypeImage != _lastItem.TypeImage)
        {
            // Debug.Log("Not same!");
            SetDefaultWorkingItem();
            return;
        }

        _isPassItem = true;
        var isPass = CompareItems(_firstItem, _lastItem);
        if(isPass) AddScore();
    }

    private void AddScore()
    {
        _score += 10;
        _scoreText.text = "Điểm: " + _score;
    }
    private void SetDefaultWorkingItem()
    {
        Invoke(nameof(ClearItem),0.2f);
    }

    private void ClearItem()
    {
        if(_firstItem) _firstItem._spriteRenderer.color = Color.white;
        if(_lastItem) _lastItem._spriteRenderer.color = Color.white;
        _firstItem = null;
        _lastItem = null;
    }
}