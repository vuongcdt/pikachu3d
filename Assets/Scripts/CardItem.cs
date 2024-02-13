using Common;
using UnityEngine;

public class CardItem : MonoBehaviour
{
    public int Id;
    public int TypeImage;
    public bool IsHas;
    private Transform _transform;
    [SerializeField] public SpriteRenderer _spriteRenderer;
    [SerializeField] public MainManager _mainManager;
    [SerializeField] private GameObject _cardItem;
    
    private void Start()
    {
        _transform = transform;
    }
    public void Init(MainManager mainManager,ImageWithType image = null)
    {
        _mainManager = mainManager;
        TypeImage = image.TypeImage;
        Id = image.Id;

        _spriteRenderer.IfNotNull(() =>
        {
            _spriteRenderer.sprite = image.Sprite;
        });
    }
    
    public Vector2 FlipAxis(Axis axis)
    {
        if (axis == Axis.Horizontal)
            return new Vector2(transform.position.y,transform.position.x);
        else
            return new Vector2(transform.position.x,transform.position.y);
    }

    void OnMouseDown()
    {
        _mainManager.SetItem(this);
    }

    public void SetShow(bool isActive)
    {
        _cardItem.SetActive(isActive);
    }

    public override string ToString()
    {
        return ($"CardItem x: {transform.position.x} y: {transform.position.y} id: {Id} type: {TypeImage}");
    }
}
