using UnityEngine;

public class PortalActivator : InteractableItem
{
	[SerializeField] private BaseItemData itemForPortal;
	[SerializeField] private GameObject endGamePanel;
	[SerializeField] private GameObject playerController;
	
	public override void Interact()
	{
		if (PlayerInventorySystem.Instance.TryConsumeItem(itemForPortal, 1))
		{
			endGamePanel.SetActive(true);
			playerController.SetActive(false);
		}
	}
}