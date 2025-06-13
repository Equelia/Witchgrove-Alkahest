using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryCellUI : MonoBehaviour,
	IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text countText;

	private IngredientType currentType;
	private bool hasItem;   // ←

	public void Setup(InventorySlot slot, Sprite sprite)
	{
		// mark that this cell actually has something
		hasItem     = true;           // ←
		currentType = slot.Type;

		icon.sprite = sprite;
		icon.enabled = sprite != null;

		if (slot.Count > 0)
		{
			countText.text    = slot.Count.ToString();
			countText.enabled = true;
		}
		else
		{
			countText.enabled = false;
		}
	}

	public void Clear()
	{
		hasItem       = false;     
		icon.enabled  = false;
		countText.enabled = false;
		Tooltip.Instance.Hide();
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		// only show if there's actually something here
		if (hasItem)
			Tooltip.Instance.Show(currentType.ToString(), eventData.position);
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		if (hasItem)
			Tooltip.Instance.Hide();
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (hasItem)
			InventorySystem.Instance.UseItem(currentType);
	}
}
