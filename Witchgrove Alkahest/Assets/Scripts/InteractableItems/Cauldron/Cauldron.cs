using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Cauldron : InteractableItem, IExternalInventoryReceiver
{
	[SerializeField] private RecipeDatabase recipeDatabase;

	[HideInInspector] public List<CellSlot> craftCellSlots;
	[HideInInspector] public List<CellSlot> resultCellSlots;
	
	[HideInInspector] public int currentWaterAmount;
	[HideInInspector] public int maxWaterAmount = 10;
	
	[SerializeField] private CauldronUI cauldronUI;

	private List<CellSlot> allSlots;

	private PotionData garbagePotion;
	private BaseItemData waterIngredient;

	public override void Interact()
	{
		base.Interact();
		InventorySystem.Instance.inventoryUI.OpenPanelByName("Cauldron");
	}

	private void Awake()
	{
		waterIngredient = ItemDatabase.Instance.GetItemById("вода");
		garbagePotion = ItemDatabase.Instance.GetPotionById("смущенноезелье");
		currentWaterAmount = 2;
		
		craftCellSlots = new List<CellSlot>();
		resultCellSlots = new List<CellSlot>();
		allSlots = new List<CellSlot>();
		
		// Initialize craft slots
		for (int i = 0; i < 8; i++)
		{
			var slot = new CellSlot();
			craftCellSlots.Add(slot);
			allSlots.Add(slot);
		}

		// Initialize result slots
		for (int i = 0; i < 6; i++)
		{
			var slot = new CellSlot();
			resultCellSlots.Add(slot);
			allSlots.Add(slot);
		}
		
		Debug.LogError(allSlots.Count.ToString());
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

			if (slot.ItemData == type && slot.Count < InventorySystem.Instance.maxStack)
			{
				int space = InventorySystem.Instance.maxStack - slot.Count;
				if (space >= count)
					return true;
				else
					count -= space;
			}
			else if (slot.Count == 0)
			{
				if (count <= InventorySystem.Instance.maxStack)
					return true;
				else
					count -= InventorySystem.Instance.maxStack;
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
			
			if (slot.ItemData == type && slot.Count < InventorySystem.Instance.maxStack)
			{
				int space = InventorySystem.Instance.maxStack - slot.Count;
				int toAdd = Mathf.Min(space, count);
				slot.Count += toAdd;
				count -= toAdd;

				if (count <= 0)
					return;
			}
			
			if (slot.Count == 0)
			{
				slot.ItemData = type;
				slot.Count = Mathf.Min(count, InventorySystem.Instance.maxStack);
				return;
			}
			
			InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
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

	public List<CellSlot> GetAllSlots() => allSlots;


	public bool TryAddOneItem(BaseItemData item)
	{
		for (int i = 0; i < allSlots.Count; i++)
		{
			var slot = allSlots[i];
			
			if (slot.ItemData == item && slot.Count < item.maxStack)
			{
				slot.Count++;
				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				cauldronUI.RefreshCellsUI();
				return true;
			}
			if (slot.ItemData == null)
			{
				slot.ItemData = item;
				slot.Count = 1;
				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				cauldronUI.RefreshCellsUI();
				return true;
			}
		}
		return false;
	}
	
	public bool TryTakeOneItem(BaseItemData item)
	{
		for (int i = 0; i < allSlots.Count; i++)
		{
			var slot = allSlots[i];
		
			if (slot.ItemData == item && slot.Count > 0)
			{
				slot.Count--;
				if (slot.Count <= 0)
					slot.ItemData = null;

				InventorySystem.Instance.inventoryUI.UpdateSlotUI(i);
				return true;
			}
		}

		return false;
	}
}