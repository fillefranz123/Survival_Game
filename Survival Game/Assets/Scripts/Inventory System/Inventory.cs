using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;



[CreateAssetMenu(fileName = "New Inventory", menuName =("Inventory Systems/Inventory"))]
[System.Serializable]
public class Inventory : ScriptableObject
{

    [SerializeField] Database database;
    [SerializeField] string savepath = "/gamedata/Inventories/inventory";
    public InventorySlot[] slots; 

    public void Initialize(int inventorySize)
    {
        slots = new InventorySlot[inventorySize];
        for (int i = 0; i < slots.Length; i++)
        {
            slots[i] = new InventorySlot(null, 0, i, this);
        }

    }

  

    public void Additem(Item item, int amount)
    {
        int slot = FindEmptySlotIndex();
        int itemSlot = -1;
        bool hasItem = FindItemOnInventory(item, out itemSlot);

        if (slot == -1 && !hasItem)
        {
            return;
        }

        else
        {
            if(hasItem)
            {
                slots[itemSlot].amount += amount;
            }

            else
            {
                slots[slot].item = item;
                slots[slot].amount += amount;
            }

               
        }
    }

    public void RemoveItem(Item item, int amount)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                if (slots[i].amount - amount <= 0)
                {
                    slots[i].item = null;
                    slots[i].amount = 0;
                }

                else
                {
                    slots[i].amount -= amount;
                }
                break;
               
            }
        }
    }

    public bool FindItemOnInventory(Item item, out int slot)
    {
        bool found = false;
       slot = -1;

        for (int i = 0;i < slots.Length; i++)
        {
            if (slots[i].item == item)
            {
                slot = i;
                found = true;
                break;
            }
        }

        return found;
    }



    public void SwapItems(int slot1, int slot2, Inventory inventory1, Inventory inventory2)
    {
        InventorySlot temp = new InventorySlot(inventory1.slots[slot1].item, inventory1.slots[slot1].amount, -1, null);
        inventory1.slots[slot1].item = slots[slot2].item;
        inventory1.slots[slot1].amount = slots[slot2].amount;

        inventory2.slots[slot2].item = temp.item;
        inventory2.slots[slot2].amount = temp.amount;
    }

    public int FindEmptySlotIndex()
    {
        int slot= -1;
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null )
            {
                slot = i;
                break;
            }
        }

        return slot;
    }

    [ContextMenu("Clear")]
    void Clear()
    {
        slots = null;
    }

    public InventoryData GetSaveData()
    {
        int[] ids = new int[slots.Length];
        int[] amount = new int[slots.Length];

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
            {
                ids[i] = -1;
                amount[i] = 0;
            }

            else
            {
                ids[i] = slots[i].item.id;
                amount[i] = slots[i].amount;
            }
        }

        InventoryData data = new InventoryData(ids, amount);

        return data;
    }

    public void Save()
    {
        string path = string.Concat(Application.persistentDataPath, savepath);

        if (!Directory.Exists(string.Concat(Application.persistentDataPath, "/gamedata/inventories")))
        {
            Directory.CreateDirectory(string.Concat(Application.persistentDataPath, "/gamedata/inventories"));
        }


        FileStream datastream = new FileStream(path, FileMode.Create, FileAccess.Write);
        Debug.Log("Saving started");


        IFormatter formatter  = new BinaryFormatter();
        formatter.Serialize(datastream, GetSaveData());
        Debug.Log("Save complete");
        datastream.Close();
    }



    public void Load()
    {
        string path = string.Concat(Application.persistentDataPath, savepath);
        InventoryData data = new InventoryData();

        if (File.Exists(path))
        {
            FileStream datastream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);

            BinaryFormatter formatter = new BinaryFormatter();
            data = (InventoryData)formatter.Deserialize(datastream);
            Debug.Log("Loaded");
            datastream.Close();
        }

        else { Debug.LogWarning("Couldn't find Save"); }





        for (int i = 0; i < slots.Length; i++)
        {
            if (data.itemIDs[i] == -1)
            {
                slots[i].item = null;
                slots[i].amount = 0;
            }

            else
            {
                slots[i].item = database.GetItemFromID(data.itemIDs[i]);
                slots[i].amount = data.amounts[i];
            }
        }
    }

    public bool CanAdditem(Item item)
    {
        int slot = -1;
        FindItemOnInventory(item, out slot);
        if (slot == -1)
        {
            slot = FindEmptySlotIndex();
            if (slot >= 0)
            {
                return true;
            }

            else
            {
                return false;
            }
        }

        else
        {
            return true;
        }


    }


}

[System.Serializable]
public struct InventorySlot
{
    public Item item;
    public int amount;
    [HideInInspector] 
    public int index;
    public Inventory inventory;
     
   public InventorySlot(Item _item, int _amount, int _index, Inventory _inventory)
   {
        item = _item;
        amount = _amount;
        index = _index;
        inventory = _inventory;
   }

    void Clear()
    {
        item = null;
        amount = 0;
    }
}


[System.Serializable]
public struct InventoryData
{
    public int[] itemIDs;
    public int[] amounts;

    public InventoryData(int[] ids, int[] _amounts)
    {
        itemIDs = ids;
        amounts = _amounts;
    }

}
