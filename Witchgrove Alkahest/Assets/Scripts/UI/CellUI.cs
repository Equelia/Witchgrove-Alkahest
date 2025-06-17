using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//TODO: Change CellUI updates
public class CellUI : MonoBehaviour,
	IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	[Header("CellUI Settings")] 
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text countText;
	
	public CellSlot SlotData { get; private set; } = new();
	public int SlotIndex { get; private set; }

	private List<CellSlot> slotList; 
	private bool hasItem;
	
	private void OnDestroy()
	{
		if (SlotData != null)
			SlotData.OnSlotChanged -= HandleSlotChanged;
	}

	public void Setup(CellSlot slot, List<CellSlot> sourceList, int index)	
	{
		if (SlotData != null)
			SlotData.OnSlotChanged -= HandleSlotChanged;

		SlotData = slot;
		slotList = sourceList;
		SlotIndex = index;
		hasItem = slot.Count > 0;

		slot.OnSlotChanged += HandleSlotChanged;
		UpdateCellUI();

	}
	
	private void HandleSlotChanged(CellSlot changedSlot)
	{
		UpdateCellUI();
	}


	public void UpdateCellUI()
	{
		if (SlotData.Count == 0 || SlotData.ItemData == null)
		{
			hasItem = false;
			icon.enabled = false;
			icon.gameObject.SetActive(false);
			countText.enabled = false;
			return;
		}
		
		hasItem = true;
		icon.sprite = SlotData.ItemData.icon;
		icon.gameObject.SetActive(true);
		icon.enabled = true;
		countText.text = SlotData.Count.ToString();
		countText.enabled = true;
		Tooltip.Instance.Hide();
	}
	
	private void TryTransferOneItem()
	{
		var receiver = InventorySystem.Instance.CurrentExternalReceiver;
		if (receiver == null || SlotData.Count == 0) return;

		bool isPlayerInv = ReferenceEquals(slotList, InventorySystem.Instance.inventorySlots);

		if (isPlayerInv)
		{
			if (receiver.TryAddOneItem(SlotData.ItemData))
			{
				SlotData.Count--;
				if (SlotData.Count == 0) SlotData.ItemData = null;
			}
		}
		else
		{
			if (InventorySystem.Instance.AddItem(SlotData.ItemData))
			{
				SlotData.Count--;
				if (SlotData.Count == 0) SlotData.ItemData = null;
			}
		}
	}

	
	#region IPoint/IDrag implementation

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hasItem && !DragManager.Instance.dragged)
			Tooltip.Instance.Show(SlotData.ItemData.displayName, eventData.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (hasItem)
			Tooltip.Instance.Hide();
	}
	
	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
		{
			TryTransferOneItem();
		}	
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (SlotData.Count <= 0) return;
		DragManager.Instance.BeginDrag(this, icon.sprite);
	}

	public void OnDrag(PointerEventData eventData)
	{
		DragManager.Instance.Drag(eventData.position);
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		DragManager.Instance.EndDrag();
	}

	public void OnDrop(PointerEventData eventData)
	{
		var dragged = DragManager.Instance.draggedItem;
		if (dragged == null || dragged.sourceSlot == this) return;

		var targetSlot = slotList[SlotIndex];
		var sourceSlot = dragged.sourceSlot.slotList[dragged.sourceIndex];

		if (targetSlot.ItemData == sourceSlot.ItemData && targetSlot.Count < targetSlot.ItemData.maxStack)
		{
			int spaceLeft = targetSlot.ItemData.maxStack - targetSlot.Count;
			int transferAmount = Mathf.Min(spaceLeft, sourceSlot.Count);

			targetSlot.Count += transferAmount;
			sourceSlot.Count -= transferAmount;

			if (sourceSlot.Count == 0)
				sourceSlot.ItemData = default;
		}
		else
		{
			var temp = new CellSlot
			{
				ItemData = targetSlot.ItemData,
				Count = targetSlot.Count
			};

			targetSlot.ItemData = sourceSlot.ItemData;
			targetSlot.Count = sourceSlot.Count;

			sourceSlot.ItemData = temp.ItemData;
			sourceSlot.Count = temp.Count;
		}
	}

	#endregion
	
}