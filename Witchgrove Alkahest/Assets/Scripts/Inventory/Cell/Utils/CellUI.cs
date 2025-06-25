using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text countText;

	public void UpdateVisuals(Cell cell)
	{
		if (cell.Count == 0 || cell.ItemData == null)
		{
			icon.enabled = false;
			icon.gameObject.SetActive(false);
			countText.enabled = false;
			return;
		}

		icon.sprite = cell.ItemData.icon;
		icon.enabled = true;
		icon.gameObject.SetActive(true);
		countText.text = cell.Count.ToString();
		countText.enabled = true;
	}

	public void Clear()
	{
		icon.enabled = false;
		icon.gameObject.SetActive(false);
		countText.enabled = false;
	}
}