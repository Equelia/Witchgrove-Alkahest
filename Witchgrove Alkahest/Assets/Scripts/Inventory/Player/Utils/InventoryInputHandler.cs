using System.Linq;
using UnityEngine;

public class InventoryInputHandler : MonoBehaviour
{
	[SerializeField] private InventoryWindowManager windowManager;

	private void Update() 
	{
		bool anySubPanelOpen = windowManager.panels.Any(entry => entry.panel.activeSelf);
		
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			if (!windowManager.IsOpen)
				windowManager.OpenInventory();
			else if (!anySubPanelOpen)
				windowManager.CloseInventory();
		}

		if (Input.GetKeyDown(KeyCode.J))
		{
			if (!windowManager.IsOpen)
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
			if (windowManager.IsOpen)
				windowManager.CloseInventory();
		}
	}
}