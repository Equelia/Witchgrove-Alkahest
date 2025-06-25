using System.Collections.Generic;
using UnityEngine;

public class Trader : InventoryProvider
{
	[Header("Items available for purchase")]
	[SerializeField] private List<TraderItemData> itemsForSale = new();
	
	[SerializeField] private PlayerData playerData;
	[SerializeField] private TraderUI traderUI;

	public List<TraderItemData> GetItemsForSale() => itemsForSale;

	public override void Interact()
	{
		base.Interact();
		PlayerInventorySystem.Instance.CurrentExternalReceiver = this;
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenPanelByName("Trader");
	}

	public bool TryBuyItem(TraderItemData item)
	{
		if (playerData.GoldAmount < item.price)
		{
			Debug.Log("[Trader] Not enough gold.");
			return false;
		}

		if (item.usable)
		{
			playerData.GoldAmount -= item.price;
			item.Use();

			if (playerData.InventoryLevel >= PlayerInventorySystem.Instance.maxInventoryLevel)
			{
				itemsForSale.Remove(item);
				traderUI.InstantiateTraderItems();
			}

			Debug.Log($"[Trader] Purchased usable item: {item.displayName} for {item.price} gold");
			return true;
		}

		if (PlayerInventorySystem.Instance.TryAddOneItem(item))
		{
			playerData.GoldAmount -= item.price;
			Debug.Log($"[Trader] Purchased: {item.displayName} for {item.price} gold");
			return true;
		}
		
		Debug.Log("[Trader] No inventory space.");
		return false;
	}

	public void SellAllPotionsInSlots()
	{
		int totalGold = 0;

		foreach (var slot in slots)
		{
			if (slot.ItemData is PotionData potion && slot.Count > 0)
			{
				int value = potion.price * slot.Count;
				totalGold += value;
				slot.Clear();
			}
		}

		if (totalGold > 0)
		{
			playerData.GoldAmount += totalGold;
			Debug.Log($"[Trader] Sold potions for {totalGold} gold.");
		}
		else
		{
			Debug.Log("[Trader] Nothing to sell.");
		}
	}
}