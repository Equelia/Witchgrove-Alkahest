using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CellUI : MonoBehaviour,
	IPointerEnterHandler, IPointerExitHandler,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text countText;

	public CellSlot SlotData { get; private set; } = new CellSlot();
	public int SlotIndex { get; private set; }
	
	private List<CellSlot> slotList;

	private ItemType currentType;
	private bool hasItem;
	

	public void Setup(CellSlot slot, List<CellSlot> sourceList, int index)
	{
		SlotData = slot;
		slotList = sourceList;
		SlotIndex = index;
		hasItem = slot.Count > 0;
		currentType = slot.Type;

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
		currentType = default;
		SlotData = new CellSlot { Type = default, Count = 0 };
		icon.enabled = false;
		countText.enabled = false;
		Tooltip.Instance.Hide();
	}
	
	public void UpdateUI()
	{
		var updatedSlot = slotList[SlotIndex];

		if (updatedSlot.Count > 0)
		{
			SlotData = updatedSlot;
			hasItem = true;
			currentType = updatedSlot.Type;
			countText.text = updatedSlot.Count.ToString();
			countText.enabled = true;

			InventorySystem.Instance.inventoryUI.iconDict.TryGetValue(updatedSlot.Type, out Sprite sprite);
			SetSprite(sprite);
		}
		else
		{
			Clear();
		}
	}
	
	public void OnPointerEnter(PointerEventData eventData)
	{
		if (hasItem && !DragManager.Instance.dragged)
			Tooltip.Instance.Show(currentType.ToString(), eventData.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (hasItem)
			Tooltip.Instance.Hide();
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

		// Совмещение стеков, если типы совпадают
		if (targetSlot.Type == sourceSlot.Type && targetSlot.Count < InventorySystem.Instance.maxStack)
		{
			int spaceLeft = InventorySystem.Instance.maxStack - targetSlot.Count;
			int transferAmount = Mathf.Min(spaceLeft, sourceSlot.Count);

			targetSlot.Count += transferAmount;
			sourceSlot.Count -= transferAmount;

			// Если источник опустел — очищаем его тип
			if (sourceSlot.Count == 0)
				sourceSlot.Type = default;
		}
		else
		{
			// Иначе просто свап
			var temp = new CellSlot
			{
				Type = targetSlot.Type,
				Count = targetSlot.Count
			};

			targetSlot.Type = sourceSlot.Type;
			targetSlot.Count = sourceSlot.Count;

			sourceSlot.Type = temp.Type;
			sourceSlot.Count = temp.Count;
		}

		dragged.sourceSlot.UpdateUI();
		UpdateUI();
	}
}
