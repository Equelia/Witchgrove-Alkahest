using System;

[Serializable]
public class Cell
{
	private BaseItemData _itemData;
	private int _count;

	public event Action<Cell> OnSlotChanged;
	public event Action<Cell> OnExternallyModified;        
	public event Action<Cell, int> OnItemAddedExternally;    
	
	public void InvokeItemAddedExternally(int amount)
	{
		OnItemAddedExternally?.Invoke(this, amount);
	}


	public BaseItemData ItemData
	{
		get => _itemData;
		set
		{
			if (_itemData == value) return;
			_itemData = value;
			OnSlotChanged?.Invoke(this);
		}
	}

	public int Count
	{
		get => _count;
		set
		{
			if (_count == value) return;

			int delta = value - _count;
			_count = value;
			OnSlotChanged?.Invoke(this);
		}
	}

	public bool IsEmpty() => _itemData == null || _count <= 0;

	public void Clear()
	{
		_itemData = null;
		_count = 0;
		OnSlotChanged?.Invoke(this);
	}

	public void ModifyCount(int delta)
	{
		Count += delta;
		if (_count <= 0)
			Clear();
	}

	/// <summary>
	/// Swaps this cell with another. Stack items if available .Triggers UI updates for both, and sound for the target.
	/// </summary>
	public void SwapWith(Cell target)
	{
		if (_itemData != null && _itemData == target._itemData)
		{
			int max = _itemData.maxStack;
			int total = _count + target._count;

			if (total <= max)
			{
				target._count = total;
				target.OnSlotChanged?.Invoke(target);
				target.OnItemAddedExternally?.Invoke(target, _count);

				Clear(); 
				return;
			}
			else
			{
				int space = max - target._count;
				if (space > 0)
				{
					_count -= space;
					target._count += space;

					OnSlotChanged?.Invoke(this);
					target.OnSlotChanged?.Invoke(target);
					target.OnItemAddedExternally?.Invoke(target, space);
					return;
				}
			}
		}
		
		(BaseItemData tempItem, int tempCount) = (_itemData, _count);
		(_itemData, _count) = (target._itemData, target._count);
		(target._itemData, target._count) = (tempItem, tempCount);

		OnSlotChanged?.Invoke(this);
		target.OnSlotChanged?.Invoke(target);
		target.OnItemAddedExternally?.Invoke(target, target._count);
	}


}