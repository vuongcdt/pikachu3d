using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public partial class MainManager
{
    private int rerenderCount = 0;

    private void RerenderGrid()
    {
        rerenderCount++;
        var listHas = _spawnedItemsList
            .Where(e => e.IsHas)
            .ToList();
        List<int> listIndex = Enumerable
            .Range(0, listHas.Count())
            .OrderBy(e => Random.Range(1, listHas.Count()))
            .ToList();

        for (var i = 0; i < listIndex.Count / 2; i++)
        {
            var oldValue = listHas[listIndex[i]];
            var newValue = listHas[listIndex[listIndex.Count - 1 - i]];

            (oldValue._spriteRenderer.sprite, newValue._spriteRenderer.sprite)
                = (newValue._spriteRenderer.sprite, oldValue._spriteRenderer.sprite);
            (oldValue.TypeImage, newValue.TypeImage)
                = (newValue.TypeImage, oldValue.TypeImage);
            (oldValue.Id, newValue.Id)
                = (newValue.Id, oldValue.Id);
        }
    }
}