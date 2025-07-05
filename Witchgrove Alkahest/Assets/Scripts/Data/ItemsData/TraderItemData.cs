using UnityEngine;

public interface IUsableItem
{
	void Use();
}


[CreateAssetMenu(menuName = "Items/TraderItemData")]
public class TraderItemData : BaseItemData, IUsableItem
{
	public bool usable;

	public void Use()
	{
		if (usable)
		{
			PlayerInventorySystem.Instance.UpgradeInventory();
			Debug.Log("Предмет использован: улучшен инвентарь");
		}
	}
}
