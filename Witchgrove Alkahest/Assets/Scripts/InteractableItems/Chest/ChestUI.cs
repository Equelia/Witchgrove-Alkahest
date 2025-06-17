using System;
using UnityEngine;
using UnityEngine.Serialization;

public class ChestUI : MonoBehaviour
{
	[Tooltip("Assign Chest CellUI components ")]
	[SerializeField] private CellUI[] chestCells;
	
	[SerializeField] private Chest chestController;

	private void Start()
	{
		for (int i = 0; i < chestCells.Length; i++)
		{
			chestCells[i].Setup(chestCells[i].SlotData, chestController.chestSlots, i);
		}
	}

	public void RefreshCellsUI()
	{
		foreach (var cell in chestCells) cell.UpdateCellUI();
	}
}