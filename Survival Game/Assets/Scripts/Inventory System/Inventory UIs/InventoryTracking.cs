using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class InventoryTracking 
{
    public static GameObject hoveredSlot;
    public static GameObject dragedSlot;
    static public Dictionary<GameObject, InventorySlot> GetSlotData = new Dictionary<GameObject, InventorySlot>();

}
