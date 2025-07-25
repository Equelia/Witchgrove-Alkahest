using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class IngredientEntryUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TextMeshProUGUI label;

	public BaseItemData ItemData { get; private set; }
	private int requiredCount;

	public void Setup(BaseItemData item, int count)
	{
		ItemData = item;
		requiredCount = count;

		if (icon != null) icon.sprite = item.icon;
		UpdateState(0);
	}

	public void UpdateState(int owned)
	{
		label.text = ItemData.displayName;
		label.color = owned >= requiredCount ? Color.green : Color.white;
	}
}