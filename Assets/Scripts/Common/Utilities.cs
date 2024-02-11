using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Common
{
    public static class Utilities
    {
        public static void  IfNotNull(this SpriteRenderer spriteRenderer,Action method)
        {
            if(spriteRenderer.IsUnityNull())
                Debug.Log("SpriteRenderer is null");
            else
                method();
        }
        
        public static void  IfNotNull(this GameObject gameObject,Action method)
        {
            if(gameObject.IsUnityNull())
                Debug.Log("GameObject is null");
            else
                method();
        }
        
        public static void  IfIsNull(this CardItem cardItem,Action method)
        {
            if (cardItem._spriteRenderer.IsUnityNull())
                method();
        }
    }
   
}