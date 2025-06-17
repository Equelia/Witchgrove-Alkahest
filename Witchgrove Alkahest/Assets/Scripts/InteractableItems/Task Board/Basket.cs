using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Basket : MonoBehaviour, IExternalInventoryReceiver
{
	[SerializeField] public TaskBoardUI taskBoardUI;
	
	[HideInInspector] public List<CellSlot> basketCells = new();
	public BasketUI basketUI;

	private void Awake()
	{
		basketCells = new List<CellSlot>(4);
		for (int i = 0; i < 4; i++)
			basketCells.Add(new CellSlot());
	}
	
	public List<CellSlot> GetAllSlots() => basketCells;


	public bool TryAddOneItem(BaseItemData item)
	{
		for (int i = 0; i < basketCells.Count; i++)
		{
			var slot = basketCells[i];

			if (slot.ItemData == item && slot.Count < item.maxStack)
			{
				slot.Count++;
				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				basketUI.RefreshCellsUI();
				taskBoardUI.UpdateAvailableItemsCount();
				return true;
			}

			if (slot.ItemData == null)
			{
				slot.ItemData = item;
				slot.Count = 1;
				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				basketUI.RefreshCellsUI();
				taskBoardUI.UpdateAvailableItemsCount();
				return true;
			}
		}

		return false;
	}
	
	public bool TryTakeOneItem(BaseItemData item)
	{
		for (int i = 0; i < basketCells.Count; i++)
		{
			var slot = basketCells[i];
		
			if (slot.ItemData == item && slot.Count > 0)
			{
				slot.Count--;
				if (slot.Count <= 0)
					slot.ItemData = null;

				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				basketUI.RefreshCellsUI();
				taskBoardUI.UpdateAvailableItemsCount();
				return true;
			}
		}

		return false;
	}

}