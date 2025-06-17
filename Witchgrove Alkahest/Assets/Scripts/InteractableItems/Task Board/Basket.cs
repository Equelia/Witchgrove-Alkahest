using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Basket : MonoBehaviour, IExternalInventoryReceiver
{
	public List<CellSlot> basketCells { get; private set; }

	private void Awake()
	{
		basketCells = new List<CellSlot>(4);
		for (int i = 0; i < 4; i++)
			basketCells.Add(new CellSlot());
	}
	
	public List<CellSlot> GetAllSlots() => basketCells;


	public bool TryAddOneItem(BaseItemData item)
	{
		foreach (var slot in basketCells)
		{
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
}