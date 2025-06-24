using System.Collections.Generic;
using UnityEngine;

public class CellController : MonoBehaviour
{
	[SerializeField] private CellUI visuals;
	[SerializeField] private CellTooltipHandler tooltipHandler;
	
	public CellSoundHandler soundHandler;

	[HideInInspector] public List<Cell> slotList;

	public Cell data { get; private set; }
	public int SlotIndex { get; private set; }

	public void Setup(Cell cell, List<Cell> list, int index, string soundName = null)
	{
		data = cell;
		slotList = list;
		SlotIndex = index;

		visuals.UpdateVisuals(data);
		soundHandler.Initialize(cell, soundName);
		tooltipHandler.SetCell(cell);

		data.OnSlotChanged += HandleSlotChanged;
		data.OnExternallyModified += HandleSlotChanged;
	}

	private void HandleSlotChanged(Cell changed)
	{
		visuals.UpdateVisuals(changed);
	}

	private void OnDestroy()
	{
		if (data != null)
		{
			data.OnSlotChanged -= HandleSlotChanged;
			data.OnExternallyModified -= HandleSlotChanged;
		}
	}
}