using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PinnedRecipeUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI recipeTitle;
	[SerializeField] private List<IngredientEntryUI> ingredientEntries = new(); 

	private Recipe currentRecipe;

	public void SetPinnedRecipe(Recipe recipe)
	{
		currentRecipe = recipe;
		gameObject.SetActive(true);
		UpdateUI();
	}

	private void UpdateUI()
	{
		recipeTitle.text = currentRecipe.result.displayName;

		foreach (var entry in ingredientEntries)
			entry.gameObject.SetActive(false);

		for (int i = 0; i < currentRecipe.ingredients.Count; i++)
		{
			var ingredient = currentRecipe.ingredients[i];
			var entry = ingredientEntries[i];
			entry.Setup(ingredient.type, ingredient.count);
			entry.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		if (currentRecipe == null) return;

		for (int i = 0; i < currentRecipe.ingredients.Count; i++)
		{
			var entry = ingredientEntries[i];
			int owned = PlayerInventorySystem.Instance.GetItemCount(entry.ItemData);
			entry.UpdateState(owned);
		}
	}
	
	public BaseItemData GetCurrentResult()
	{
		return currentRecipe?.result;
	}
	
	public bool HavePinnedRecipe()
	{
		return currentRecipe != null;
	}


	public void Hide()
	{
		currentRecipe = null;
		gameObject.SetActive(false);
	}

}