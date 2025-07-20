using System.Linq;
using UnityEngine;

public class InventoryInputHandler : MonoBehaviour
{
	[SerializeField] private InventoryWindowManager windowManager;
	[SerializeField] private ObjectInteractor objectInteractor;

	private void Update() 
	{
		
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!windowManager.IsInventoryOpen)
				windowManager.OpenInventory();
			else if (!windowManager.AnySubPanelOpen)
				windowManager.CloseInventory();
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			if (windowManager.IsInventoryOpen & windowManager.AnySubPanelOpen)
			{
				windowManager.CloseInventory();
				windowManager.ClosePanelByName("RecipeBook");
			}
			else if (!windowManager.IsInventoryOpen)
			{
				windowManager.OpenInventory();
				windowManager.OpenPanelByName("RecipeBook");
			}
			else if (!windowManager.AnySubPanelOpen)
			{
				windowManager.CloseInventory();
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
	}
}