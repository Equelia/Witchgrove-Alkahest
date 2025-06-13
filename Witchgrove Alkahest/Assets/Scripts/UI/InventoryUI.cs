using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Helper class to bind an IngredientType to a Sprite in the Inspector.
/// </summary>
[Serializable]
public class IngredientIcon
{
    public IngredientType Type;
    public Sprite Icon;
}

/// <summary>
/// Handles toggling the inventory panel and updating slot visuals.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI Panel")]
    [Tooltip("Root panel to show/hide")]
    [SerializeField] private GameObject panel;

    [Header("Slot Cells")]
    [Tooltip("Assign InventoryCellUI components ")]
    [SerializeField] private InventoryCellUI[] cells;

    [Header("Icons for Each Item Type")]
    [Tooltip("Drag & drop sprites for each IngredientType")]
    [SerializeField] private List<IngredientIcon> icons = new List<IngredientIcon>();

    // Internal lookup for fast icon retrieval
    private Dictionary<IngredientType, Sprite> iconDict;

    private void Awake()
    {
        iconDict = new Dictionary<IngredientType, Sprite>();
        foreach (var entry in icons)
        {
            if (!iconDict.ContainsKey(entry.Type))
                iconDict.Add(entry.Type, entry.Icon);
        }
    }

    private void Start()
    {
        InventorySystem.Instance.OnInventoryChanged += RefreshUI;
        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            bool nowOpen = !panel.activeSelf;
            panel.SetActive(nowOpen);

            // if we just closed it, hide any lingering tooltip
            if (!nowOpen && Tooltip.Instance != null)
                Tooltip.Instance.Hide();
        }
    }

    /// <summary>
    /// Refresh all slots to match the current inventory state.
    /// </summary>
    private void RefreshUI()
    {
        var slots = InventorySystem.Instance.GetSlots();

        for (int i = 0; i < cells.Length; i++)
        {
            if (i < slots.Count && slots[i].Count > 0)
            {
                var slot = slots[i];
                iconDict.TryGetValue(slot.Type, out Sprite sprite);
                cells[i].Setup(slot, sprite);
            }
            else
            {
                cells[i].Clear();
            }
        }
    }
}
