using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class InventoryUI : MonoBehaviour,  IPointerEnterHandler, IPointerExitHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [Header("Data")]
    public Inventory inventory;
    public Database database;

    [Header("UI Elements")]
    public GameObject slotPrefab;
    public GameObject uiScreen;
    public Image dragDisplay;
    public GameObject[] uiSlots;

    //References
    //[HideInInspector]
    public SaveSystem saveSystem;
    public static List<InventoryUI> interfaces = new List<InventoryUI>();
    public Dictionary<GameObject, InventorySlot> getLocalSlot = new Dictionary<GameObject, InventorySlot>();

    

    private void Awake()
    {
        saveSystem = FindObjectOfType<SaveSystem>();

        
       
    }



    // Start is called before the first frame update
    public virtual void Start()
    {
        interfaces.Add(this);
        
    }

   public virtual void Update()
    {

        if (Input.GetKeyDown(KeyCode.K))
        {
            inventory.Save();
        }


        if (Input.GetKeyDown(KeyCode.L))
        {
            inventory.Load();
            UpdateAllSlots();
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            UpdateAllSlots();
        }
    }

    public void UpdateAllSlots()
    {
        for (int i = 0; i < inventory.slots.Length; i++)
        {
            UpdateDisplay(inventory.slots[i]);
        }
    }

   

    public void UpdateDisplay(InventorySlot slot)
    {
        TextMeshProUGUI amountText = uiSlots[slot.index].GetComponentInChildren<TextMeshProUGUI>();
        Image image = amountText.GetComponentInParent<Image>();

       
        if (slot.item == null)
        {
            image.enabled = false;
            image.sprite = null;
            amountText.enabled = false;
        }

        else
        {
            image.enabled = true;
            image.sprite = slot.item.ui;
            amountText.enabled = true;
            amountText.text = slot.amount.ToString();

        }
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        for (int i = 0; i < eventData.hovered.Count; i++)
        {
            if (!eventData.hovered[i].TryGetComponent<Canvas>(out Canvas canvas) && canvas == null)
            {
                InventoryTracking.hoveredSlot = eventData.hovered[i].transform.parent.gameObject;
                break;
            }

        }

    }


    public void OnPointerExit(PointerEventData eventData)
    {
        InventoryTracking.hoveredSlot = null;
    }

   


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (InventoryTracking.hoveredSlot != null && InventoryTracking.GetSlotData.ContainsKey(InventoryTracking.hoveredSlot))
        {
            if (inventory.slots[InventoryTracking.GetSlotData[InventoryTracking.hoveredSlot].index].item != null)
            {
                dragDisplay.enabled = true;
                dragDisplay.sprite = inventory.slots[InventoryTracking.GetSlotData[InventoryTracking.hoveredSlot].index].item.ui;

            }
           
            InventoryTracking.dragedSlot = InventoryTracking.hoveredSlot;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragDisplay.transform.position = Input.mousePosition;
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        
        if (InventoryTracking.dragedSlot != null && InventoryTracking.hoveredSlot != null && InventoryTracking.GetSlotData.ContainsKey(InventoryTracking.hoveredSlot) && InventoryTracking.GetSlotData.ContainsKey(InventoryTracking.dragedSlot) && InventoryTracking.GetSlotData.ContainsValue(InventoryTracking.GetSlotData[InventoryTracking.hoveredSlot]) && InventoryTracking.GetSlotData.ContainsValue(InventoryTracking.GetSlotData[InventoryTracking.hoveredSlot]))
        {
            InventorySlot slot1 = InventoryTracking.GetSlotData[InventoryTracking.hoveredSlot];
            InventorySlot slot2 = InventoryTracking.GetSlotData[InventoryTracking.dragedSlot];
            inventory.SwapItems(slot1.index, slot2.index, slot1.inventory, slot2.inventory);

            if (slot1.inventory == slot2.inventory)
            {
                UpdateAllSlots();

            }

            else
            {
                for (int i = 0; i < interfaces.Count; i++)
                {
                    interfaces[i].UpdateAllSlots();
                }
            }

            
        }
        dragDisplay.enabled = false;
        InventoryTracking.dragedSlot = null;
    }

    
}
