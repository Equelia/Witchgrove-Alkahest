using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Chest : InteractableItem, IExternalInventoryReceiver
{
	[SerializeField] private ChestUI chestUI;
	
	[HideInInspector] public List<CellSlot> chestSlots = new();
	[HideInInspector] public string ChestId;
	
	private void Awake()
	{
		chestSlots = new List<CellSlot>(8);
		for (int i = 0; i < 8; i++)
			chestSlots.Add(new CellSlot());
	}

	public override void Interact()
	{
		base.Interact();
		InventorySystem.Instance.inventoryUI.OpenPanelByName("Chest");
	}

	public List<CellSlot> GetAllSlots() => chestSlots;


	public bool TryAddOneItem(BaseItemData item)
	{
		for (int i = 0; i < chestSlots.Count; i++)
		{
			var slot = chestSlots[i];
			
			if (slot.ItemData == item && slot.Count < item.maxStack)
			{
				slot.Count++;
				return true;
			}
			if (slot.ItemData == null)
			{
				slot.ItemData = item;
				slot.Count = 1;
				return true;
			}
		}
		return false;
	}
	
	public void ClearSlots()
	{
		foreach (var s in chestSlots)
		{
			s.ItemData = null;
			s.Count = 0;
		}
	}
	
	public void AddToFirstEmpty(BaseItemData item, int count)
	{
		for (int i = 0; i < count; i++)
			TryAddOneItem(item);
	}
}