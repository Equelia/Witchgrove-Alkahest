using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class CellDragHandler : MonoBehaviour,
	IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler, IPointerClickHandler
{
	private CellController cellController;

	private void Awake()
	{
		cellController = GetComponent<CellController>();
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (cellController.data.IsEmpty())
			return;

		DragManager.Instance.BeginDrag(cellController, cellController.data.ItemData.icon);
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
		if (cellController.IsLocked) return;
		
		var dragged = DragManager.Instance.draggedItem;
		if (dragged == null || dragged.sourceSlot == cellController)
			return;

		var targetSlot = cellController.slotList[cellController.SlotIndex];
		var sourceSlot = dragged.sourceSlot.slotList[dragged.sourceIndex];

		sourceSlot.SwapWith(targetSlot); 
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (eventData.button == PointerEventData.InputButton.Right)
			TryTransferOneItem();
	}

	
	private void TryTransferOneItem()
	{
		var receiver = PlayerInventorySystem.Instance.CurrentExternalReceiver;
		if (receiver == null || cellController.data.Count == 0) return;

		bool isPlayerInventory = ReferenceEquals(cellController.slotList, PlayerInventorySystem.Instance.GetAllSlots());
		BaseItemData item = cellController.data.ItemData;

		if (isPlayerInventory)
		{
			if (receiver.TryAddOneItem(item))
			{
				cellController.data.ModifyCount(-1);
				if (cellController.data.Count == 0)
					cellController.data.ItemData = null;

				if (receiver is IExternalInventoryReceiver trackedReceiver)
				{
					var target = trackedReceiver.GetLastModifiedSlot();
					target?.InvokeItemAddedExternally(1); 
				}
			}
		}
		else
		{
			if (PlayerInventorySystem.Instance.TryAddOneItem(item))
			{
				cellController.data.ModifyCount(-1);
				if (cellController.data.Count == 0)
					cellController.data.ItemData = null;

				var target = PlayerInventorySystem.Instance.GetAllSlots()
					.FirstOrDefault(s => s.ItemData == item && s.Count > 0);
				target?.InvokeItemAddedExternally(1);
			}
		}
	}
}