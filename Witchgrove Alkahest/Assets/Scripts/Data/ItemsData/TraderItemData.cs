using UnityEngine;

[CreateAssetMenu(menuName = "Items/TraderItemData")]
public class TraderItemData : BaseItemData
{
	public int price;

	public bool usable;
	
	public void Use()
	{
		PlayerInventorySystem.Instance.UpgradeInventory();
	}
}