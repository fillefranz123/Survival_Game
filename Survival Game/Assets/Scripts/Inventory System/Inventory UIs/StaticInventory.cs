using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StaticInventory : InventoryUI
{

    [Header("Dimensions")]
    [SerializeField] Vector2Int inventoryLayout = new Vector2Int(4, 5);
    [SerializeField] Vector2 spacing = new Vector2(40, 60);


    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        CreateUI();

        for (int i = 0; i < inventory.slots.Length; i++)
        {
            InventoryTracking.GetSlotData.Add(uiSlots[i], inventory.slots[i]);

        }

        inventory.Additem(database.items[0], 2);
        inventory.Additem(database.items[0], 1);
        inventory.Additem(database.items[1], 1);
        UpdateAllSlots();
        
        dragDisplay.enabled = false;
    }

    void CreateUI()
    {

        int inventorySize = inventoryLayout.x * inventoryLayout.y;
        uiSlots = new GameObject[inventorySize];

        inventory.Initialize(inventorySize);
        for (int i = 0; i < inventorySize; i++)
        {
            GameObject slot = Instantiate(slotPrefab, uiScreen.transform);
            uiSlots[i] = slot;
            slot.gameObject.name = $"Slot {i}";


        }

        Vector2Int invertedInventorySize = new Vector2Int(inventoryLayout.y, inventoryLayout.x);
        int xSpaces = invertedInventorySize.x + 2;
        int ySpaces = invertedInventorySize.y + 2;

        GridLayoutGroup grid = uiScreen.GetComponent<GridLayoutGroup>();
        RectTransform rectTransform = uiScreen.GetComponent<RectTransform>();

        Vector2 slotSize = grid.cellSize;

        float uiWidth = slotSize.x * invertedInventorySize.x + xSpaces * spacing.x;
        float uiHeight = slotSize.y * invertedInventorySize.y + ySpaces * spacing.y + spacing.y;

        rectTransform.sizeDelta = new Vector2(uiHeight, uiWidth);

        grid.spacing = new Vector2(spacing.x, spacing.y);
        grid.padding = new RectOffset(Mathf.RoundToInt(spacing.x), Mathf.RoundToInt(spacing.x), Mathf.RoundToInt(spacing.y), Mathf.RoundToInt(spacing.y));



    }



    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }
}
