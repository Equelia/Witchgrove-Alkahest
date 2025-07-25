using UnityEngine;

public class Animal : InteractableItem
{
	[Header("Required item to exchange for gold")]
	[SerializeField] private BaseItemData requiredItem;
	[SerializeField] private string soundName;
	[SerializeField] private int goldAmount;
	
	public override void Interact()
	{
		SoundManager.Instance.PlaySoundOnceUntilComplete(soundName);

		if (PlayerInventorySystem.Instance.TryConsumeItem(requiredItem, 1))
		{
			PlayerInventorySystem.Instance.playerData.GoldAmount += goldAmount;
			SoundManager.Instance.PlaySound("GoldChange");
		}
	}
}