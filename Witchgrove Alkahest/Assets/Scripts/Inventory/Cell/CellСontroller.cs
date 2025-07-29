using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
	[SerializeField] private CellUI visuals;
	[SerializeField] private CellTooltipHandler tooltipHandler;
	[SerializeField] private Image lockImage;


	public CellSoundHandler soundHandler;

	[HideInInspector] public List<Cell> slotList;

	public Cell data { get; private set; }
	public int SlotIndex { get; private set; }
	public bool IsLocked { get; private set; }

	public void Setup(Cell cell, List<Cell> list, int index, bool isLocked = false, string soundName = null)
	{
		data = cell;
		slotList = list;
		SlotIndex = index;
		IsLocked = isLocked;

		lockImage.gameObject.SetActive(IsLocked);

		// всегда обновляем визуал, даже если заблокировано
		visuals.UpdateVisuals(data);

		// подсказка всегда включена, но не будет срабатывать без предмета
		tooltipHandler.SetCell(cell);

		if (!IsLocked)
		{
			soundHandler.Initialize(cell, soundName);

			data.OnSlotChanged += HandleSlotChanged;
			data.OnExternallyModified += HandleSlotChanged;
		}
	}



	private void HandleSlotChanged(Cell changed)
	{
		if (!IsLocked)
			visuals.UpdateVisuals(changed);
	}

	private void OnDestroy()
	{
		if (data != null && !IsLocked)
		{
			data.OnSlotChanged -= HandleSlotChanged;
			data.OnExternallyModified -= HandleSlotChanged;
		}
	}
}