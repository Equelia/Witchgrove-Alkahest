// Cauldron.cs
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Handles crafting in a cauldron: consumes ingredients + water and adds result directly to player inventory.
/// </summary>
public class Cauldron : InteractableItem, IExternalInventoryReceiver
{
    [Header("Cauldron Settings")] 
    [Tooltip("List of all crafting recipes")] 
    [SerializeField] private RecipeDatabase recipeDatabase;

    [Tooltip("If true, ingredient order matters for matching recipes")] 
    [SerializeField] private bool useSpecificOrder = false; 

    public List<CellSlot> craftCellSlots { get; private set; }

    private PotionData garbagePotion;

    public override void Interact()
    {
        base.Interact();
        InventorySystem.Instance.inventoryUI.OpenPanelByName("Cauldron");
        InventorySystem.Instance.CurrentExternalReceiver = this;
    }

    private void Awake()
    {
        garbagePotion = ItemDatabase.Instance.GetPotionById("смущенноезелье");

        craftCellSlots = new List<CellSlot>();
        for (int i = 0; i < 8; i++)
            craftCellSlots.Add(new CellSlot()); // Initialize craft slots
    }

    /// <summary>
    /// Attempts to craft a potion: checks water, matches recipe, checks inventory space, consumes ingredients + water,
    /// and adds result directly into player inventory. Fails if inventory is full.
    /// </summary>
    public void TryCraft()
    {
        if (craftCellSlots.All(slot => slot.Count == 0)) 
            return;

        Recipe matchedRecipe = null;
        foreach (var recipe in recipeDatabase.recipes)
        {
            if (Matches(recipe))
            {
                matchedRecipe = recipe;
                break;
            }
        }

        BaseItemData resultType;
        int resultCount;
        if (matchedRecipe != null)
        {
            resultType = matchedRecipe.result;
            resultCount = matchedRecipe.resultCount;
        }
        else
        {
            resultType = garbagePotion;
            resultCount = 1;
        }

        if (!HasInventorySpace(resultType, resultCount)) 
        {
            Debug.LogWarning("[Cauldron] Not enough inventory space. Craft cancelled.");
            return;
        }

        if (matchedRecipe != null)
        {
            Debug.Log("[Cauldron] Recipe matched: " + matchedRecipe.result.displayName);
            ConsumeIngredients(matchedRecipe);
            for (int i = 0; i < resultCount; i++)
                InventorySystem.Instance.AddItem(resultType);
            
            SoundManager.Instance.PlaySound("CauldronCraft");
        }
        else
        {
            Debug.LogWarning("[Cauldron] Craft failed. Making Смущенное зелье!");
            ClearCraftSlots();
            InventorySystem.Instance.AddItem(resultType); 
            SoundManager.Instance.PlaySound("CauldronCraft");
        }
    }

    private bool Matches(Recipe recipe)
    {
        var nonEmpty = craftCellSlots.Where(s => s.Count > 0).ToList();
        if (nonEmpty.Count != recipe.ingredients.Count)
            return false;

        if (useSpecificOrder)
        {
            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                var expected = recipe.ingredients[i];
                var actual = nonEmpty[i];
                if (actual.ItemData != expected.type || actual.Count < expected.count)
                    return false;
            }
            return true;
        }
        else 
        {
            var slotsCopy = new List<CellSlot>(nonEmpty);
            foreach (var expected in recipe.ingredients)
            {
                var match = slotsCopy.FirstOrDefault(s => s.ItemData == expected.type && s.Count >= expected.count);
                if (match == null) return false;
                slotsCopy.Remove(match);
            }
            return true;
        }
    }

    private void ConsumeIngredients(Recipe recipe)
    {
        var nonEmpty = craftCellSlots.Where(s => s.Count > 0).ToList();
        foreach (var expected in recipe.ingredients)
        {
            var slot = nonEmpty.First(s => s.ItemData == expected.type);
            slot.Count -= expected.count;
            if (slot.Count <= 0)
            {
                slot.ItemData = null;
                slot.Count = 0;
            }
            nonEmpty.Remove(slot);
        }
    }

    private void ClearCraftSlots()
    {
        foreach (var slot in craftCellSlots)
        {
            slot.ItemData = null;
            slot.Count = 0;
        }
    }

    private bool HasInventorySpace(BaseItemData type, int count) 
    {
        var inv = InventorySystem.Instance.inventorySlots;
        foreach (var slot in inv)
        {
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

    public List<CellSlot> GetAllSlots() => craftCellSlots;

    public bool TryAddOneItem(BaseItemData item)
    {
        foreach (var slot in craftCellSlots)
        {
            if (slot.ItemData == item && slot.Count < item.maxStack)
            {
                slot.Count++;
                SoundManager.Instance.PlaySound("CauldronDrop");
                return true;
            }
            if (slot.Count == 0)
            {
                slot.ItemData = item;
                slot.Count = 1;
                SoundManager.Instance.PlaySound("CauldronDrop");
                return true;
            }
        }
        return false;
    }
}
