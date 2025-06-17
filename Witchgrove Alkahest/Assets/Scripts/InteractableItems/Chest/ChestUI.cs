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
		var slots = chestController.GetAllSlots();  

		for (int i = 0; i < chestCells.Length && i < slots.Count; i++)
		{
			chestCells[i].Setup(slots[i], slots, i);
		}
	}
}