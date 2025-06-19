using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[System.Serializable]
public class PanelEntry
{
	public string name;
	public GameObject panel;
}

/// <summary>
/// Handles the inventory UI and listens to slot changes.
/// </summary>
public class InventoryUI : MonoBehaviour
{
	[Header("UI Elements")] [Tooltip("UI Elements panel's to show/hide")] [SerializeField]
	private GameObject mainInventoryPanel;

	[SerializeField] private List<PanelEntry> panels;

	[Header("Inventory Slot Cells")] [Tooltip("Assign inventory CellUI components ")] [SerializeField]
	private CellUI[] cells;

	[SerializeField, Space(10f)] private CellUI[] trashBinCell;
	[SerializeField, Space(10f)] private Image trashSlotRadialTimer;

	public bool IsOpen => mainInventoryPanel.activeSelf;

	private void OnEnable()
	{
		foreach (var slot in InventorySystem.Instance.inventorySlots)
			slot.OnSlotChanged += HandleSlotChanged;

		foreach (var slot in InventorySystem.Instance.trashBinSlots)
			slot.OnSlotChanged += HandleTrashBinSlotChanged;
	}

	private void OnDisable()
	{
		foreach (var slot in InventorySystem.Instance.inventorySlots)
			slot.OnSlotChanged -= HandleSlotChanged;
		
		foreach (var slot in InventorySystem.Instance.trashBinSlots)
			slot.OnSlotChanged -= HandleTrashBinSlotChanged;
	}

	private void Start()
	{
		CloseInventory();

		var slots = InventorySystem.Instance.inventorySlots;
		var trashBinSlot = InventorySystem.Instance.trashBinSlots;
		for (int i = 0; i < cells.Length && i < slots.Count; i++)
		{
			cells[i].Setup(slots[i], slots, i);
		}

		for (int i = 0; i < trashBinCell.Length && i < trashBinSlot.Count; i++)
		{
			trashBinCell[i].Setup(trashBinSlot[i], trashBinSlot, i);
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

		FillTrashBinTimer();
	}

	public void FillTrashBinTimer()
	{
		trashSlotRadialTimer.fillAmount = InventorySystem.Instance.GetTrashProgress();
	}

	private void HandleTrashBinSlotChanged(CellSlot slot)
	{
		bool has = slot.Count > 0 && slot.ItemData != null;
		if (has)
		{
			// когда в мусорку что-то положили — запускаем таймер
			InventorySystem.Instance.StartTrashTimer();
			trashSlotRadialTimer.gameObject.SetActive(true);
		}
		else
		{
			// когда забрали/очистили — сбрасываем
			InventorySystem.Instance.CancelTrashTimer();
			trashSlotRadialTimer.gameObject.SetActive(false);
		}
	}


	private void HandleSlotChanged(CellSlot slot)
	{
		int index = InventorySystem.Instance.inventorySlots.IndexOf(slot);
		if (index >= 0 && index < cells.Length)
			cells[index].UpdateCellUI();
	}

	public void CloseInventory()
	{
		mainInventoryPanel.SetActive(false);

		foreach (var entry in panels)
			entry.panel.SetActive(false);

		InventorySystem.Instance.CurrentExternalReceiver = null;
		Tooltip.Instance.Hide();
	}

	public void OpenInventory()
	{
		mainInventoryPanel.SetActive(true);
	}

	public void OpenPanelByName(string panelName)
	{
		foreach (var entry in panels)
		{
			entry.panel.SetActive(entry.name == panelName);
		}
	}
}