using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
	public List<IngredientData> ingredients;
	public List<PotionData> potions;
	
	private Dictionary<string, BaseItemData> itemMap;

	public void OnEnable()
	{
		// Построим единую карту по id -> объект
		itemMap = new Dictionary<string, BaseItemData>();

		if (ingredients != null)
		{
			foreach (var item in ingredients)
			{
				if (item != null)
					itemMap[item.id] = item;
			}
		}

		if (potions != null)
		{
			foreach (var item in potions)
			{
				if (item != null)
					itemMap[item.id] = item;
			}
		}
	}

	// From Resources/ItemDatabase.asset
	public static ItemDatabase Instance => Resources.Load<ItemDatabase>("ItemDatabase");

	/// <summary>
	///  Get item by ID
	/// </summary>
	public BaseItemData GetItemById(string id)
	{
		itemMap.TryGetValue(id, out var item);
		return item;
	}

	/// <summary>
	/// Get potion by ID
	/// </summary>
	public PotionData GetPotionById(string id)
	{
		return GetItemById(id) as PotionData;
	}
}