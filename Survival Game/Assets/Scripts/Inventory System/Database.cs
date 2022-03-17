using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Database", menuName = "Inventory Systems/Database")]
[System.Serializable]
public class Database : ScriptableObject
{
    public Item[] items;

    [ContextMenu("Update")]
    void UpdateDatabase()
    {
        for (int i = 0; i < items.Length; i++)
        {
            items[i].id = i;
        }
    }

    public Item GetItemFromID(int id)
    {
        Item item = items[id];
        return item;
    }
}
