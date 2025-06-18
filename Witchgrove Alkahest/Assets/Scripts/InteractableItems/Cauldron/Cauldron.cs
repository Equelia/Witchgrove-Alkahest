using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cauldron : InteractableItem, IExternalInventoryReceiver
{
	[SerializeField] private RecipeDatabase recipeDatabase;

	public List<CellSlot> craftCellSlots { get; private set; }
	public List<CellSlot> resultCellSlots { get; private set; }
	
	[HideInInspector] public int currentWaterAmount;
	[HideInInspector] public int maxWaterAmount = 10;
	
	[SerializeField] private CauldronUI cauldronUI;


	private PotionData garbagePotion;
	private BaseItemData waterIngredient;

	public override void Interact()
	{
		base.Interact();
		InventorySystem.Instance.inventoryUI.OpenPanelByName("Cauldron");
		InventorySystem.Instance.CurrentExternalReceiver = this;  
	}

	private void Awake()
	{
		currentWaterAmount = 2;
		
		waterIngredient = ItemDatabase.Instance.GetItemById("вода");
		garbagePotion = ItemDatabase.Instance.GetPotionById("смущенноезелье");
		
		craftCellSlots = new List<CellSlot>();
		resultCellSlots = new List<CellSlot>();
		
		for (int i = 0; i < 8; i++) craftCellSlots.Add(new CellSlot()); // Initialize craft slots
		for (int i = 0; i < 6; i++) resultCellSlots.Add(new CellSlot()); // Initialize result slots
	}
	
	public void TryCraft()
	{
		if (CheckForEmptyIngredientSlots())
			return;
		
		if (currentWaterAmount <= 0)
		{
			Debug.Log("[Cauldron] Нет воды в котле. Крафт невозможен.");
			return;
		}
		
		bool matched = false;
		
		foreach (var recipe in recipeDatabase.recipes)
		{
			if (Matches(recipe))
			{
				if (!HasSpaceForResult(recipe.result, recipe.resultCount))
				{
					Debug.LogWarning("[Cauldron] Нет места для результата. Крафт отменён.");
					return;
				}
				
				matched = true;
				Debug.Log("[Cauldron] Recipe matched: " + recipe.result);

				ConsumeIngredients(recipe);

				AddToResultSlot(recipe.result, recipe.resultCount);
				currentWaterAmount--;
				break;
			}
		}
		
		if (!matched)
		{
			if (!HasSpaceForResult(garbagePotion, 1))
			{
				Debug.LogWarning("[Cauldron] Нет места даже для смущённого зелья. Крафт отменён.");
				return;
			}
			
			Debug.LogWarning("[Cauldron] Craft failed. Making Смущенное зелье!");

			ClearCraftSlots();

			AddToResultSlot(garbagePotion, 1);
			currentWaterAmount--;
		}
	}

	private bool CheckForEmptyIngredientSlots()
	{
		foreach (var slot in craftCellSlots)
		{
			if (slot.Count > 0)
			{
				return false;
			}
		}
		
		return true;
	}
	
	private bool HasSpaceForResult(BaseItemData type, int count)
	{
		for (int i = 0; i < resultCellSlots.Count; i++)
		{
			var slot = resultCellSlots[i];

			if (slot.ItemData == type && slot.Count < type.maxStack)
			{
				int space = type.maxStack - slot.Count;
				if (space >= count)
					return true;
				count -= space;
			}
			else if (slot.Count == 0)
			{
				if (count <= type.maxStack)
					return true;
				count -= type.maxStack;
			}
		}
		return false;
	}



	/// <summary>
	/// Check for possible ingredients matches
	/// </summary>
	private bool Matches(Recipe recipe)
	{
		List<CellSlot> nonEmptySlots = new List<CellSlot>();

		// Get non empty slots
		foreach (var slot in craftCellSlots)
		{
			if (slot.Count > 0)
				nonEmptySlots.Add(slot);
		}

		// Check for empty slots count for match
		if (nonEmptySlots.Count != recipe.ingredients.Count)
			return false;

		// Check for match one by one
		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			var expected = recipe.ingredients[i];
			var actual = nonEmptySlots[i];

			if (actual.ItemData != expected.type || actual.Count < expected.count)
				return false;
		}

		return true;
	}


	private void ConsumeIngredients(Recipe recipe)
	{
		List<CellSlot> nonEmptySlots = new List<CellSlot>();

		foreach (var slot in craftCellSlots)
		{
			if (slot.Count > 0)
				nonEmptySlots.Add(slot);
		}

		for (int i = 0; i < recipe.ingredients.Count; i++)
		{
			var expected = recipe.ingredients[i];
			var slot = nonEmptySlots[i];

			if (slot.ItemData == expected.type)
			{
				slot.Count -= expected.count;
				if (slot.Count <= 0)
				{
					slot.ItemData = null;
					slot.Count = 0;
				}
			}
		}
	}

	private void ClearCraftSlots()
	{
		foreach (var slot in craftCellSlots)
		{
			slot.ItemData = default;
			slot.Count = 0;
		}
	}
	
	private void AddToResultSlot(BaseItemData type, int count)
	{
		for(int i = 0; i < resultCellSlots.Count; i++)
		{
			var slot = resultCellSlots[i];
			
			if (slot.ItemData == type && slot.Count < slot.ItemData.maxStack)
			{
				int space = slot.ItemData.maxStack - slot.Count;
				int toAdd = Mathf.Min(space, count);
				slot.Count += toAdd;
				count -= toAdd;

				if (count <= 0)
					return;
			}
			
			if (slot.Count == 0)
			{
				slot.ItemData = type;
				slot.Count = Mathf.Min(count, slot.ItemData.maxStack);
				return;
			}
		}

		Debug.LogWarning($"[Cauldron] No space for result item: {type}");
	}

	public bool TryAddWater()
	{
		if (currentWaterAmount < maxWaterAmount)
		{
			if (InventorySystem.Instance.TryConsumeItem(waterIngredient, 1))
			{
				currentWaterAmount++;
				return true;
			}
			else
			{
				Debug.Log("Нет воды в инвентаре.");
				return false;
			}
		}
		else
		{
			Debug.Log("Котёл уже заполнен водой.");
			return false;
		}
	}

	public List<CellSlot> GetAllSlots() => craftCellSlots.Concat(resultCellSlots).ToList();

	public bool TryAddOneItem(BaseItemData item)
	{
		foreach (var slot in GetAllSlots())
		{
			if (slot.ItemData == item && slot.Count < item.maxStack)
			{
				slot.Count++;
				return true;
			}
			if (slot.ItemData == null)
			{
				slot.ItemData = item;
				slot.Count = 1;
				return true;
			}
		}
		return false;
	}
}