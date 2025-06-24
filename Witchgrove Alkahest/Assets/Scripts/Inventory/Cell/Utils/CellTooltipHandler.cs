using UnityEngine;
using UnityEngine.EventSystems;

public class CellTooltipHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
	private Cell cell;

	public void SetCell(Cell c) => cell = c;

	public void OnPointerEnter(PointerEventData eventData)
	{
		if (cell != null && cell.ItemData != null && cell.Count > 0 && !DragManager.Instance.dragged)
			Tooltip.Instance.Show(cell.ItemData.displayName, eventData.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Tooltip.Instance.Hide();
	}
}