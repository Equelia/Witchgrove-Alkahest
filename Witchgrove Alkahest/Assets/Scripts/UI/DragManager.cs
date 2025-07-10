using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton for handle dragging object
/// </summary>
public class DragManager : MonoBehaviour
{
	public static DragManager Instance;

	public GameObject draggedIconObject;
	public Image draggedIcon;

	public DragItemData draggedItem;
	public Cell currentDraggedCell;

	
	[HideInInspector] public bool dragged = false;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		draggedIconObject.SetActive(false);
	}

	public void BeginDrag(CellController cell, Sprite icon)
	{
		if (cell == null || cell.data.Count == 0) return;
		
		dragged = true;
		
		draggedItem = new DragItemData
		{
			sourceSlot = cell,
			sourceIndex = cell.SlotIndex
		};

		currentDraggedCell = cell.data;
		draggedIcon.sprite = icon;
		draggedIconObject.SetActive(true);
	}

	public void Drag(Vector2 pos)
	{
		draggedIconObject.transform.position = pos;
	}

	public void EndDrag()
	{
		dragged = false;
		draggedItem = null;
		currentDraggedCell = null;
		draggedIconObject.SetActive(false);
	}
}