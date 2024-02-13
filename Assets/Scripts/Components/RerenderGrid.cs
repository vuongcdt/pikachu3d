using System.Linq;
using UnityEngine;

public partial class MainManager
{
    private void RerenderGrid()
    {
        CheckTypeItem(_spawnedItemsList);
        Debug.Log(_spawnedItemsList.Count(e=>e.IsHas));
        
        var hasItemList = _spawnedItemsList
            .Where(e => e.IsHas)
            .ToList();
        CheckTypeItem(hasItemList);
        Debug.Log(hasItemList.Count());
        
        var hasItemListRamdom = _spawnedItemsList
            .Where(e => e.IsHas)
            .OrderBy(e => Random.Range(0, _width * _height))
            .ToList();
        CheckTypeItem(hasItemListRamdom);
        Debug.Log(hasItemListRamdom.Count);
        
        // for (var i = 0; i < hasItemList.Count(); i++)
        // {
        //     hasItemList[i]._spriteRenderer.sprite = hasItemListRamdom[i]._spriteRenderer.sprite;
        //     hasItemList[i].TypeImage = hasItemListRamdom[i].TypeImage;
        // }
        
        int index = 0;
        foreach (var cardItem in _spawnedItemsList)
        {
            if (cardItem.IsHas)
            {
                cardItem.TypeImage = hasItemListRamdom[index].TypeImage;
                cardItem._spriteRenderer.sprite = hasItemListRamdom[index]._spriteRenderer.sprite;
                index++;
            }
        }

        CheckTypeItem(_spawnedItemsList);
        Debug.Log(_spawnedItemsList.Count(e=>e.IsHas));
    }
}