using System.Linq;
using UnityEngine;

public class InventoryInputHandler : MonoBehaviour
{
	[SerializeField] private InventoryWindowManager windowManager;
	[SerializeField] private ObjectInteractor objectInteractor;
	[SerializeField] private PinnedRecipeUI pinnedRecipe;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!windowManager.IsInventoryOpen)
			{
				windowManager.OpenInventory();
			}
			else if (!windowManager.AnySubPanelOpen || IsOnlyPanelOpen("RecipeBook"))
			{
				windowManager.CloseInventory();
			}
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			bool recipeBookIsOpen = windowManager.IsPanelOpen("RecipeBook");

			if (recipeBookIsOpen)
			{
				windowManager.ClosePanelByName("RecipeBook");
				
				if (!windowManager.AnySubPanelOpen && windowManager.IsInventoryOpen)
					windowManager.CloseInventory();
			}
			else
			{
				windowManager.OpenPanelByName("RecipeBook");
			}
		}


		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (windowManager.AnySubPanelOpen || windowManager.IsInventoryOpen)
			{
				windowManager.CloseInventory();
			}
			else if (!windowManager.IsMenuOpen)
			{
				windowManager.OpenMainMenu();
			}
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (windowManager.AnySubPanelOpen && !objectInteractor.BlockInteractionThisFrame)
			{
				windowManager.CloseInventory();
				objectInteractor.BlockInteractionThisFrame = true;
			}
		}

		bool hasPinned = pinnedRecipe.HavePinnedRecipe();
		bool recipeBookOpen = IsPanelOpen("RecipeBook");
		bool anyOtherPanelOpen = windowManager.AnySubPanelOpen && !recipeBookOpen;

		bool shouldShowPinned = hasPinned && (recipeBookOpen || !windowManager.IsInventoryOpen && !windowManager.AnySubPanelOpen);

		pinnedRecipe.gameObject.SetActive(shouldShowPinned && !anyOtherPanelOpen);
	}
	
	private bool IsPanelOpen(string panelName)
	{
		return windowManager.panels.Any(p => p.name == panelName && p.panel.activeSelf);
	}

	
	private bool IsOnlyPanelOpen(string panelName)
	{
		return windowManager.panels.Count(p => p.panel.activeSelf) == 1 &&
		       windowManager.panels.Any(p => p.name == panelName && p.panel.activeSelf);
	}
}
