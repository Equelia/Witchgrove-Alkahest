using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BookRecipeBinder : MonoBehaviour
{
	[SerializeField] private Book book;
	[SerializeField] private PinnedRecipeUI pinnedUI;
	[SerializeField] private Button pinButton;
	[SerializeField] private TextMeshProUGUI pinButtonLabel;
	
	[Header("Receipts")]
	[SerializeField] private RecipeDatabase recipeDatabase;

	[Tooltip("Recipe results titles in the correct order (one per book spread)")]
	[SerializeField] private List<string> resultNamesBySpread = new();
	
	private Recipe currentPinnedRecipe;
	
	private const string PinnedRecipeKey = "PinnedRecipeName";

	private void Start()
	{
		string savedName = PlayerPrefs.GetString(PinnedRecipeKey, "");
		if (!string.IsNullOrWhiteSpace(savedName))
		{
			var recipe = recipeDatabase.recipes.Find(r =>
				r.result != null &&
				r.result.displayName.Trim().Equals(savedName, System.StringComparison.OrdinalIgnoreCase));

			if (recipe != null)
			{
				pinnedUI.SetPinnedRecipe(recipe);
				currentPinnedRecipe = recipe;
			}
		}
		
		gameObject.SetActive(false);
	}

	private void OnEnable()
	{
		pinButton.onClick.AddListener(OnPinButtonClicked);
	}

	private void OnDisable()
	{
		pinButton.onClick.RemoveListener(OnPinButtonClicked);
	}
	
	private int lastKnownSpreadIndex = -1;

	private void Update()
	{
		int currentSpread = book.currentPage / 2;

		if (currentSpread != lastKnownSpreadIndex)
		{
			lastKnownSpreadIndex = currentSpread;
			UpdateButtonLabel();
		}
	}


	private void OnPinButtonClicked()
	{
		int spreadIndex = book.currentPage / 2;

		// Defensive: invalid spread
		if (spreadIndex < 0 || spreadIndex >= resultNamesBySpread.Count)
			return;

		string targetName = resultNamesBySpread[spreadIndex];

		// Defensive: empty or whitespace
		if (string.IsNullOrWhiteSpace(targetName))
			return;

		// Find matching recipe
		var recipe = recipeDatabase.recipes.Find(r =>
			r.result != null &&
			r.result.displayName.Trim().Equals(targetName.Trim(), System.StringComparison.OrdinalIgnoreCase));

		if (recipe == null)
		{
			Debug.LogWarning($"Recipe '{targetName}' not found in RecipeDatabase.");
			return;
		}

		// Toggle logic
		if (pinnedUI.HavePinnedRecipe() && pinnedUI.GetCurrentResult() == recipe.result)
		{
			// Unpin if already active
			pinnedUI.Hide();
			currentPinnedRecipe = null;
			PlayerPrefs.DeleteKey(PinnedRecipeKey);

		}
		else
		{
			// Pin and display
			pinnedUI.SetPinnedRecipe(recipe);
			currentPinnedRecipe = recipe;
			PlayerPrefs.SetString(PinnedRecipeKey, recipe.result.displayName.Trim());
			PlayerPrefs.Save();
			GoalController.Instance.TriggerGoalProgress(GoalConditionType.PinRecipe);
		}

		UpdateButtonLabel();
	}

	public void UpdateButtonLabel()
	{
		int spreadIndex = book.currentPage / 2;

		if (spreadIndex < 0 || spreadIndex >= resultNamesBySpread.Count)
		{
			pinButtonLabel.text = "Закрепить";
			return;
		}

		string targetName = resultNamesBySpread[spreadIndex];

		var recipe = recipeDatabase.recipes.Find(r =>
			r.result != null &&
			r.result.displayName.Trim().Equals(targetName.Trim(), System.StringComparison.OrdinalIgnoreCase));

		if (recipe != null && pinnedUI.HavePinnedRecipe() && pinnedUI.GetCurrentResult() == recipe.result)
		{
			pinButtonLabel.text = "Открепить";
		}
		else
		{
			pinButtonLabel.text = "Закрепить";
		}
	}
}