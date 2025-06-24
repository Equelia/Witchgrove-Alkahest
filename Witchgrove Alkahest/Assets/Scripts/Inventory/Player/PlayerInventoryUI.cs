using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Handles the inventory UI and listens to slot changes.
/// </summary>
public class PlayerInventoryUI : MonoBehaviour
{
	[Header("Inventory Slot Cells")]
	[SerializeField] private CellController[] inventoryCells;
	
	public InventoryWindowManager inventoryWindowManager;

	private void Start()
	{
		var inventorySlots = PlayerInventorySystem.Instance.GetAllSlots();
		for (int i = 0; i < inventoryCells.Length && i < inventorySlots.Count; i++)
		{
			inventoryCells[i].Setup(inventorySlots[i], inventorySlots, i);
		}
	}
}