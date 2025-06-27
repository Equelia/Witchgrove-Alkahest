using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PanelEntry
{
	public string name;
	public GameObject panel;
}

public class InventoryWindowManager : MonoBehaviour
{
	[SerializeField] private GameObject mainInventoryPanel;
	public List<PanelEntry> panels;
	
	public event Action OnInventoryClosed;

	public bool IsOpen => mainInventoryPanel.activeSelf;

	private void Start()
	{
		CloseInventory();
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
		PlayerInventorySystem.Instance.CurrentExternalReceiver = null;
		Tooltip.Instance.Hide();
		OnInventoryClosed?.Invoke();
	}

	public void OpenPanelByName(string panelName)
	{
		foreach (var entry in panels)
			entry.panel.SetActive(entry.name == panelName);
	}

	public void ClosePanelByName(string panelName)
	{
		foreach (var entry in panels)
			if (entry.panel.name == panelName)
				entry.panel.SetActive(false);
	}
}