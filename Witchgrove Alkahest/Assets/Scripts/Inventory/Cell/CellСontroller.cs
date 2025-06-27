using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
	[SerializeField] private CellUI visuals;
	[SerializeField] private CellTooltipHandler tooltipHandler;
	[SerializeField] private Image lockImage;
	[SerializeField] private GameObject itemCountHolder;


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
		itemCountHolder.SetActive(!IsLocked); 


		if (!IsLocked)
		{
			visuals.UpdateVisuals(data);
			soundHandler.Initialize(cell, soundName);
			tooltipHandler.SetCell(cell);

			data.OnSlotChanged += HandleSlotChanged;
			data.OnExternallyModified += HandleSlotChanged;
		}
		else
		{
			visuals.Clear();
			tooltipHandler.enabled = false;
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