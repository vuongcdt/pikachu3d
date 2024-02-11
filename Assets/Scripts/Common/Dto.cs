using UnityEngine;

namespace Common
{
    public class ImageWithType
    {
        public Sprite Sprite;
        public int TypeImage;
        public int Id;

        public ImageWithType(){}
        public ImageWithType(Sprite sprite, int typeImage)
        {
            Sprite = sprite;
            TypeImage = typeImage;
        }
    }

    public class ItemStore
    {
        public float X;
        public float Y;
        public int Id;
        public int TypeImage;
        public bool IsHas;

        public ItemStore()
        {
        }

        public ItemStore(int id, int typeImage)
        {
            Id = id;
            TypeImage = typeImage;
        }
        public Vector2 FlipAxis(Axis axis)
        {
            if (axis == Axis.Horizontal)
                return new Vector2(Y,X);
            else
                return new Vector2(X,Y);
        }
        public ItemStore(float x, float y, int id, int typeImage, bool isHas)
        {
            X = x;
            Y = y;
            Id = id;
            TypeImage = typeImage;
            IsHas = isHas;
        }

        public override string ToString()
        {
            return ($"ItemStore x: {X} y: {Y} id: {Id}");
        }
    }
}