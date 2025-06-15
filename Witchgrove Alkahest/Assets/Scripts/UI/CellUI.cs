using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

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
	private BaseItemData itemData;
	private bool hasItem;

	public void Setup(CellSlot slot, List<CellSlot> sourceList, int index)
	{
		SlotData = slot;
		slotList = sourceList;
		SlotIndex = index;
		hasItem = slot.Count > 0;
		itemData = slot.ItemData;

		countText.text = slot.Count > 0 ? slot.Count.ToString() : "";
		countText.enabled = slot.Count > 0;
	}

	private void SetSprite(Sprite sprite)
	{
		icon.sprite = sprite;
		icon.gameObject.SetActive(sprite != null);
		icon.enabled = sprite != null;
	}

	public void Clear()
	{
		hasItem = false;
		itemData = default;
		SlotData = new CellSlot { ItemData = default, Count = 0 };
		icon.enabled = false;
		countText.enabled = false;
		Tooltip.Instance.Hide();
	}

	public void UpdateCellUI()
	{
		var updatedSlot = slotList[SlotIndex];

		if (updatedSlot.Count > 0)
		{
			SlotData = updatedSlot;
			hasItem = true;
			itemData = updatedSlot.ItemData;
			countText.text = updatedSlot.Count.ToString();
			countText.enabled = true;

			SetSprite(itemData.icon);
		}
		else
		{
			Clear();
		}
	}
	
	private void TryGiveOneItemToExternal()
	{
		if (SlotData == null || SlotData.ItemData == null || SlotData.Count <= 0) return;

		var externalReceiver = InventorySystem.Instance.CurrentExternalReceiver;
		if (externalReceiver == null) return;

		if (!externalReceiver.CanReceiveItem(SlotData.ItemData)) return;

		if (externalReceiver.ReceiveItem(SlotData.ItemData, 1))
		{
			SlotData.Count -= 1;
			if (SlotData.Count == 0)
			{
				SlotData.ItemData = null;
			}
			UpdateCellUI();
			InventorySystem.Instance.inventoryUI.UpdateSlotUI(SlotIndex);
		}
	}


	#region IPoint/IDrag implementation

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hasItem && !DragManager.Instance.dragged)
			Tooltip.Instance.Show(itemData.ToString(), eventData.position);
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
			TryGiveOneItemToExternal();
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

		if (targetSlot.ItemData == sourceSlot.ItemData && targetSlot.Count < InventorySystem.Instance.maxStack)
		{
			int spaceLeft = InventorySystem.Instance.maxStack - targetSlot.Count;
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

		dragged.sourceSlot.UpdateCellUI();
		UpdateCellUI();
	}

	#endregion
	
}