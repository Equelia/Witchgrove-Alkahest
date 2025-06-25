using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TraderItemUI : MonoBehaviour
{
	[SerializeField] private Image icon;
	[SerializeField] private TMP_Text nameText;
	[SerializeField] private TMP_Text priceText;
	[SerializeField] private Button buyButton;

	private TraderItemData itemData;
	private Trader trader;

	public void Setup(TraderItemData item, Trader traderContext)
	{
		itemData = item;
		trader = traderContext;

		icon.sprite = item.icon;
		nameText.text = item.displayName;
		priceText.text = item.price.ToString();

		buyButton.onClick.RemoveAllListeners();
		buyButton.onClick.AddListener(() => trader.TryBuyItem(itemData));
	}
}