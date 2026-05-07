using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Te_InventorySlot
{
    public ItemData item;
    public int count;

    public Te_InventorySlot(ItemData newItem, int newCount)
    {
        item = newItem;
        count = newCount;
    }

    public void Clear() { item = null; count = 0; }
}
