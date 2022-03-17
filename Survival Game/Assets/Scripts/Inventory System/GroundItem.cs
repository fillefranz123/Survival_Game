using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroundItem : MonoBehaviour
{
    [SerializeField] Item item;
    [SerializeField] int amount = 1;

    [SerializeField] StaticInventory inventory;
    [SerializeField] Hotbar hotbar;


    // Start is called before the first frame update
    void Start()
    {
        inventory = FindObjectOfType<StaticInventory>();
        hotbar = FindObjectOfType<Hotbar>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Called through a message from PlayerInput
    void Interact()
    {
        if (hotbar.inventory.CanAdditem(item))
        {
            hotbar.inventory.Additem(item, amount);
            hotbar.UpdateAllSlots();
            Destroy(gameObject);
        }

        else if(inventory.inventory.CanAdditem(item))
        {
            inventory.inventory.Additem(item,amount);
            hotbar.UpdateAllSlots();
            Destroy(gameObject);
        }
        
        
    }
}
