using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Handles crafting in a cauldron: consumes ingredients and adds result directly to player inventory.
/// </summary>
public class Cauldron : InventoryProvider
{
    [SerializeField] private RecipeDatabase recipeDatabase;
    [SerializeField] private bool useSpecificOrder = false;
    
    [SerializeField] private ParticleSystem[] bublesParticles;
    
    [Header("Tutorial")]
    [SerializeField] private TutorialUIGroup tutorialUIGroup;

    private PotionData garbagePotion;

    public override void Awake()
    {
        base.Awake();
        garbagePotion = ItemDatabase.Instance.GetPotionById("смущенноезелье");
    }

    public override void Interact()
    {
        base.Interact();
        PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenPanelByName("Cauldron");
        tutorialUIGroup?.Show();
    }

    public void TryCraft()
    {
        if (slots.All(slot => slot.Count == 0))
            return;

        Recipe matchedRecipe = recipeDatabase.recipes.FirstOrDefault(Matches);

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
            ConsumeIngredients(matchedRecipe);
            for (int i = 0; i < resultCount; i++)
                PlayerInventorySystem.Instance.TryAddOneItem(resultType);
        }
        else
        {
            ClearAllSlots();
            PlayerInventorySystem.Instance.TryAddOneItem(resultType);
        }

        PlayCraftEffectsAsync().Forget();
    }

    private bool Matches(Recipe recipe)
    {
        var nonEmpty = slots.Where(s => s.Count > 0).ToList();
        if (nonEmpty.Count != recipe.ingredients.Count)
            return false;

        if (useSpecificOrder)
        {
            for (int i = 0; i < recipe.ingredients.Count; i++)
            {
                if (nonEmpty.Count <= i) return false;
                var expected = recipe.ingredients[i];
                var actual = nonEmpty[i];
                if (actual.ItemData != expected.type || actual.Count < expected.count)
                    return false;
            }
            return true;
        }
        else
        {
            var copy = new List<Cell>(nonEmpty);
            foreach (var expected in recipe.ingredients)
            {
                var match = copy.FirstOrDefault(s => s.ItemData == expected.type && s.Count >= expected.count);
                if (match == null) return false;
                copy.Remove(match);
            }
            return true;
        }
    }

    private void ConsumeIngredients(Recipe recipe)
    {
        var nonEmpty = slots.Where(s => s.Count > 0).ToList();
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

    private bool HasInventorySpace(BaseItemData type, int count)
    {
        var inv = PlayerInventorySystem.Instance.GetAllSlots();
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
    
    
    private async UniTaskVoid PlayCraftEffectsAsync()
    {
        foreach (var ps in bublesParticles)
            ps.Play();

        AudioClip clip = SoundManager.Instance.PlaySound("CauldronCraft");

        if (clip != null)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(clip.length), cancellationToken: this.GetCancellationTokenOnDestroy());
        }

        foreach (var ps in bublesParticles)
            ps.Stop();
    }
}
