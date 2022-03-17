using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Item", menuName = "Inventory Systems/Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string itemName;
    public int id;
    public Sprite ui;
    public bool stackable;
    public enum ItemTypes { weapon, armor,resource, food, structure}
    public ItemTypes type;
}
