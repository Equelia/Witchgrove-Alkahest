using UnityEngine;

public class Chest : InventoryProvider
{
	[SerializeField] private ChestUI chestUI;

	public string ChestId;

	public override void Interact()
	{
		base.Interact();
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenPanelByName("Chest");
	}
}