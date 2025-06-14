using System;
using System.Collections.Generic;
using UnityEngine;

public class CauldronUI : MonoBehaviour
{
	[Tooltip("Assign craft CellUI components ")]
	[SerializeField] private CellUI[] craftCells;

	[Tooltip("Assign craft CellUI components ")]
	[SerializeField] private CellUI[] resultCells;
	
	[Tooltip("Assign Cauldron component")]
	[SerializeField] private Cauldron cauldronController;
	
	private InventoryUI inventoryUI => InventorySystem.Instance.inventoryUI;

	private void Start()
	{
		for (int i = 0; i < craftCells.Length; i++)
		{
			craftCells[i].Setup(craftCells[i].SlotData, cauldronController.craftCellSlots, i);
		}
		for (int i = 0; i < resultCells.Length; i++)
		{
			resultCells[i].Setup(resultCells[i].SlotData, cauldronController.resultCellSlots, i);
		}
	}
}
