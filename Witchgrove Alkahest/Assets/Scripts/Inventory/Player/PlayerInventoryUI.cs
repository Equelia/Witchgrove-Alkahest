using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryUI : MonoBehaviour
{
	[Header("Inventory Slot Cells")]
	[SerializeField] private CellController[] inventoryCells;

	public InventoryWindowManager inventoryWindowManager;

	private void Start()
	{
		RefreshUI(); 
	}

	private void OnEnable()
	{
		PlayerInventorySystem.Instance.playerData.OnInventoryLevelChanged += RefreshUI;
	}

	private void OnDisable()
	{
		PlayerInventorySystem.Instance.playerData.OnInventoryLevelChanged -= RefreshUI;
	}

	public void RefreshUI()
	{
		var inventorySlots = PlayerInventorySystem.Instance.GetAllSlots();
		int unlockedCellsCount = PlayerInventorySystem.Instance.GetUnlockedSlotCount();

		for (int i = 0; i < inventoryCells.Length; i++)
		{
			bool isUnlocked = i < unlockedCellsCount;

			if (inventoryCells[i] != null)
			{
				inventoryCells[i].gameObject.SetActive(isUnlocked);

				if (isUnlocked)
				{
					if (i < inventorySlots.Count)
					{
						inventoryCells[i].Setup(inventorySlots[i], inventorySlots, i);
					}
					else
					{
						Debug.LogWarning($"[PlayerInventoryUI] Нет слота с индексом {i}, всего {inventorySlots.Count}");
					}
				}
			}
		}
	}

}