using System;
using System.Collections.Generic;
using UnityEngine;


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
    
    public bool IsOpen => panel.activeSelf;

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
        InventorySystem.Instance.CurrentExternalReceiver = null;
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
