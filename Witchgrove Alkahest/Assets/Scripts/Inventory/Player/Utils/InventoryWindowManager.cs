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
	[SerializeField] private GameObject mainMenuPanel;
	[SerializeField] private GameObject darkBackground;

	public List<PanelEntry> panels;

	public event Action OnInventoryClosed;

	public bool IsInventoryOpen => mainInventoryPanel.activeSelf || darkBackground.activeSelf;
	public bool IsMenuOpen => mainMenuPanel.activeSelf;

	private void Start()
	{
		CloseInventory();
	}

	private void UpdateDarkBGState()
	{
		if (darkBackground == null) return;

		bool anyVisible = mainInventoryPanel.activeSelf || panels.Any(p => p.panel.activeSelf);
		darkBackground.SetActive(anyVisible);
	}


	public void OpenInventory()
	{
		mainInventoryPanel.SetActive(true);
		UpdateDarkBGState();
	}

	public void OpenMainMenu()
	{
		mainMenuPanel.SetActive(true);
	}

	public void CloseMainMenu()
	{
		mainMenuPanel.SetActive(false);
	}

	public void CloseInventory()
	{
		mainInventoryPanel.SetActive(false);
		foreach (var entry in panels)
			entry.panel.SetActive(false);
		PlayerInventorySystem.Instance.CurrentExternalReceiver = null;
		Tooltip.Instance.Hide();
		OnInventoryClosed?.Invoke();
		UpdateDarkBGState();
	}

	public void OpenPanelByName(string panelName)
	{
		foreach (var entry in panels)
		{
			if (entry.name == panelName)
			{
				entry.panel.SetActive(true);
				break;
			}
		}

		UpdateDarkBGState();
	}

	public void ClosePanelByName(string panelName)
	{
		foreach (var entry in panels)
		{
			if (entry.name == panelName && entry.panel.activeSelf)
			{
				entry.panel.SetActive(false);
				break;
			}
		}

		UpdateDarkBGState();
	}
	
	
	public bool IsPanelOpen(string panelName)
	{
		return panels.Any(p => p.name == panelName && p.panel.activeSelf);
	}

	
	public bool AnySubPanelOpen => panels.Any(p => p.panel.activeSelf);
}