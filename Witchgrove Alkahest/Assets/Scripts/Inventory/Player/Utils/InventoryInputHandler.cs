using System.Linq;
using UnityEngine;

public class InventoryInputHandler : MonoBehaviour
{
	[SerializeField] private InventoryWindowManager windowManager;
	[SerializeField] private ObjectInteractor objectInteractor;

	private void Update() 
	{
		bool anySubPanelOpen = windowManager.panels.Any(entry => entry.panel.activeSelf);
		
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!windowManager.IsInventoryOpen)
				windowManager.OpenInventory();
			else if (!anySubPanelOpen)
				windowManager.CloseInventory();
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			if (windowManager.IsInventoryOpen & anySubPanelOpen)
			{
				windowManager.CloseInventory();
				windowManager.ClosePanelByName("RecipeBook");
			}
			else if (!windowManager.IsInventoryOpen)
			{
				windowManager.OpenInventory();
				windowManager.OpenPanelByName("RecipeBook");
			}
			else if (!anySubPanelOpen)
			{
				windowManager.CloseInventory();
			}
		}

		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (windowManager.IsInventoryOpen || anySubPanelOpen)
				windowManager.CloseInventory();
			else if (!windowManager.IsMenuOpen)
				windowManager.OpenMainMenu();
			else
				windowManager.CloseMainMenu();
		}

		if (Input.GetKeyDown(KeyCode.E))
		{
			if (anySubPanelOpen)
			{
				windowManager.CloseInventory();
				objectInteractor.BlockInteractionThisFrame = true;
			}
		}
	}
}