using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for every Inventory
/// </summary>
public interface IExternalInventoryReceiver
{
	List<Cell> GetAllSlots();
	bool TryAddOneItem(BaseItemData item);
	Cell GetLastModifiedSlot();
}

/// <summary>
/// Base class for all objects that have inventory in it
/// </summary>
public abstract class InventoryProvider : InteractableItem, IExternalInventoryReceiver
{
	[Header("Inventory Settings")]
	[SerializeField] protected int slotCount = 8;
    
	protected List<Cell> slots;
	private Cell lastModifiedCell;

	public virtual void Awake()
	{
		slots = new List<Cell>(slotCount);
		for (int i = 0; i < slotCount; i++)
			slots.Add(new Cell());
	}

	public override void Interact()
	{
		base.Interact();
		PlayerInventorySystem.Instance.CurrentExternalReceiver = this;
	}

	public virtual List<Cell> GetAllSlots() => slots;

	public void ClearAllSlots()
	{
		foreach (var slot 
		         in slots)
		{
			slot.Clear();
		}
	}

	public bool TryAddOneItem(BaseItemData item)
	{
		if (item is TraderItemData traderItem && traderItem.usable)
		{
			traderItem.Use();
			return true;
		}
		
		foreach (var slot in slots)
		{
			if (slot.ItemData == item && slot.Count < item.maxStack)
			{
				slot.Count++;
				lastModifiedCell = slot;
				return true;
			}

			if (slot.IsEmpty())
			{
				slot.ItemData = item;
				slot.Count = 1;
				lastModifiedCell = slot;
				return true;
			}
		}
		
		Debug.Log("Inventory is full");
		return false;
	}

	public Cell GetLastModifiedSlot() => lastModifiedCell;
	
	public void AddToFirstEmpty(BaseItemData item, int count)
	{
		for (int i = 0; i < count; i++)
			TryAddOneItem(item);
	}
}