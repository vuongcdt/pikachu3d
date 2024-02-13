using System.Linq;
using UnityEngine;

public partial class MainManager
{
    private void RerenderGrid()
    {
        var hasItemList = _spawnedItemsList
            .Where(e => e.IsHas)
            .ToArray();
        // CheckTypeItem();

        var hasItemListRamdom = _spawnedItemsList
            .Where(e => e.IsHas)
            .OrderBy(e => Random.Range(0, hasItemList.Count()))
            .ToArray();
        
        for (var i = 0; i < hasItemList.Length; i++)
        {
            hasItemList[i]._spriteRenderer.sprite = hasItemListRamdom[i]._spriteRenderer.sprite;
            hasItemList[i].TypeImage = hasItemListRamdom[i].TypeImage;
        }
        
    }
}