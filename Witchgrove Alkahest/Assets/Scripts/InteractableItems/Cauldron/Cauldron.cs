using System;
using System.Collections.Generic;
using UnityEngine;

public class Cauldron : InteractableItem
{
	[HideInInspector] public List<CellSlot> craftCellSlots;
	[HideInInspector] public List<CellSlot> resultCellSlots;

	
	public override void Interact()
	{
		base.Interact();
		InventorySystem.Instance.inventoryUI.OpenCauldron();
	}
	
	private void Awake()
	{
		// Initialize empty craft slots
		craftCellSlots = new List<CellSlot>(8);
		for (int i = 0; i < 8; i++)
		{
			craftCellSlots.Add(new CellSlot { Type = default, Count = 0 });
		}
		
		// Initialize empty result slots
		resultCellSlots = new List<CellSlot>(6);
		for (int i = 0; i < 6; i++)
		{
			resultCellSlots.Add(new CellSlot { Type = default, Count = 0 });
		}
	}
}