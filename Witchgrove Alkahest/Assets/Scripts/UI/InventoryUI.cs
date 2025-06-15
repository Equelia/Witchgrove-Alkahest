using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to bind an IngredientType to a Sprite in the Inspector.
/// </summary>
[Serializable]
public class IngredientIcon
{
    public ItemType Type;
    public Sprite Icon;
}

/// <summary>
/// Handles toggling the inventory panel and updating slot visuals.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI Elements")]
    [Tooltip("UI Elements panel's to show/hide")]
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject cauldronPanel;

    [Header("Inventory Slot Cells")]
    [Tooltip("Assign inventory CellUI components ")]
    [SerializeField] private CellUI[] cells;

    [Header("Icons for Each Item Type")]
    [Tooltip("Drag & drop sprites for each IngredientType")]
    [SerializeField] private List<IngredientIcon> icons = new List<IngredientIcon>();
    
    public bool IsOpen => panel.activeSelf;

    // Internal lookup for fast icon retrieval
    public Dictionary<ItemType, Sprite> iconDict;

    private void Awake()
    {
        iconDict = new Dictionary<ItemType, Sprite>();
        foreach (var entry in icons)
        {
            if (!iconDict.ContainsKey(entry.Type))
                iconDict.Add(entry.Type, entry.Icon);
        }
    }

    private void Start()
    {
        CloseInventory();
        
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i].Setup(cells[i].SlotData, InventorySystem.Instance.inventorySlots, i);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (!IsOpen)
                panel.SetActive(true);
            else
                CloseInventory();
        }
    }

    public void OpenInventory()
    {
        panel.SetActive(true);
    }

    private void CloseInventory()
    {
        panel.SetActive(false);
        cauldronPanel.SetActive(false);
        Tooltip.Instance.Hide();
    }

    public void OpenCauldron()
    {
        cauldronPanel.SetActive(true);
    }

    public void UpdateSlotUI(int index)
    {
        if (index >= 0 && index < cells.Length)
            cells[index].UpdateCellUI();
    }
}
