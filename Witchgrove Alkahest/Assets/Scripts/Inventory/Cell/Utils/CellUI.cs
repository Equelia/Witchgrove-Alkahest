using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text countText;
	[SerializeField] private GameObject itemCountHolder;

	public void UpdateVisuals(Cell cell)
	{
		bool hasItem = cell.Count > 0 && cell.ItemData != null;

		icon.enabled = hasItem;
		icon.gameObject.SetActive(hasItem);
		countText.enabled = hasItem;
		itemCountHolder.SetActive(hasItem);

		if (hasItem)
		{
			icon.sprite = cell.ItemData.icon;
			countText.text = cell.Count.ToString();
		}
	}

	public void Clear()
	{
		icon.enabled = false;
		icon.gameObject.SetActive(false);
		countText.enabled = false;
		itemCountHolder.SetActive(false);
	}
}