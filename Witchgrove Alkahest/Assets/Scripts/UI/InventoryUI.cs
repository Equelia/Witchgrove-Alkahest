using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class PanelEntry
{
    public string name;
    public GameObject panel;
}

/// <summary>
/// Handles toggling the inventory panel and updating slot visuals.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [FormerlySerializedAs("panel")]
    [Header("UI Elements")]
    [Tooltip("UI Elements panel's to show/hide")]
    [SerializeField] private GameObject mainInventoryPanel;
    [SerializeField] private List<PanelEntry> panels;

    [Header("Inventory Slot Cells")]
    [Tooltip("Assign inventory CellUI components ")]
    [SerializeField] private CellUI[] cells;
    
    public bool IsOpen => mainInventoryPanel.activeSelf;

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
                mainInventoryPanel.SetActive(true);
            else
                CloseInventory();
        }
    }

    public void OpenInventory()
    {
        mainInventoryPanel.SetActive(true);
    }


    public void CloseInventory()
    {
        mainInventoryPanel.SetActive(false);

        foreach (var entry in panels)
            entry.panel.SetActive(false);

        InventorySystem.Instance.CurrentExternalReceiver = null;
        Tooltip.Instance.Hide();
    }
    
    public void OpenPanelByName(string panelName)
    {
        foreach (var entry in panels)
        {
            entry.panel.SetActive(entry.name == panelName);
        }
    }

    public void UpdateSlotUI(int index)
    {
        if (index >= 0 && index < cells.Length)
            cells[index].UpdateCellUI();
    }
}
