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
	
	public bool dragged = false;

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;
		DontDestroyOnLoad(gameObject);
		draggedIconObject.SetActive(false);
	}

	public void BeginDrag(CellUI cell, Sprite icon)
	{
		if (cell == null || cell.SlotData.Count == 0) return;
		
		dragged = true;
		
		draggedItem = new DragItemData
		{
			sourceSlot = cell,
			sourceIndex = cell.SlotIndex
		};

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
		draggedIconObject.SetActive(false);
	}
}