using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : InventoryUI
{
    [SerializeField] GameObject selectionIndictator;

    [Header("Dimensions")]
    [SerializeField] int hotbarSize = 4;
    [SerializeField] int spacing = 50;
    [SerializeField] int slotSize = 100;
    [SerializeField] int verticalPadding = 50;

    public Item heldItem = null;

    public int heldSlot = 0;

    //interaction Scripts
    [SerializeField] StructurePlacer structurePlacer;


    void Awake()
    {
        CreateUI();
        for (int i = 0; i < uiSlots.Length; i++)
        {
            getLocalSlot.Add(uiSlots[i], inventory.slots[i]);
        }
    }

    void CreateUI()
    {
        inventory.Initialize(hotbarSize);
        uiSlots = new GameObject[hotbarSize];


        for (int i = 0; i < hotbarSize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, uiScreen.transform);
            slot.gameObject.name = $"Slot {i}";
            InventoryTracking.GetSlotData.Add(slot, inventory.slots[i]);

            uiSlots[i] = slot;

        }

        HorizontalLayoutGroup layoutGroup = uiScreen.GetComponent<HorizontalLayoutGroup>();
        layoutGroup.spacing = spacing;
        layoutGroup.padding = new RectOffset(spacing, spacing, verticalPadding, verticalPadding);

        int spaces = hotbarSize + 2;

        float uiHeight = slotSize + 2 * verticalPadding;
        float uiWidth = slotSize * hotbarSize + spaces * spacing;

        RectTransform rectTransform = uiScreen.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(uiWidth, uiHeight);
    }

    void SwitchHeldItem()
    {
        //Scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            int scrollDelta = -Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10);
            heldSlot += scrollDelta;

            if (heldSlot >= hotbarSize)
            {
                heldSlot = 0;
            }

            else if(heldSlot< 0)
            {
                heldSlot = hotbarSize-1;
            }

            selectionIndictator.transform.position = uiSlots[heldSlot].transform.position;
            heldItem = inventory.slots[heldSlot].item;
            OnHeldItemChanged();
        }

        //Hotkeys
        else
        {
            int pressedNumber = GetPressedNumber();

            if (pressedNumber > 0 && pressedNumber <= hotbarSize)
            {
                heldSlot = pressedNumber - 1;
                selectionIndictator.transform.position = uiSlots[heldSlot].transform.position;
                heldItem = inventory.slots[heldSlot].item;
                OnHeldItemChanged();
            }
        }
    }

    int GetPressedNumber()
    {
        for (int number = 0; number <= 9 ; number++)
        {
            if (Input.GetKeyDown(number.ToString()))
            {
                return number;
            }
        }

        return -1;
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();
        inventory.Additem(database.items[5], 99);
        inventory.Additem(database.items[3], 99);
        inventory.Additem(database.items[4], 99);
        UpdateAllSlots();
        structurePlacer = FindObjectOfType<StructurePlacer>();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();

       

        SwitchHeldItem();

        
    }

    void OnHeldItemChanged()
    {
        DisableAllInteractionScripts();

        if (heldItem != null && heldItem.type == Item.ItemTypes.structure )
        {
            structurePlacer.enabled = true;
            structurePlacer.SetHeldItem(heldItem);
        }
    }

    void DisableAllInteractionScripts()
    {
        structurePlacer.enabled = false;

    }

}
