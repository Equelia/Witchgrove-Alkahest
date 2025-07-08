using DG.Tweening;
using UnityEngine;

public class Chest : InventoryProvider
{
	public string ChestId;
	
	[Header("Tutorial")]
	[SerializeField] private TutorialUIGroup tutorialUIGroup;

	[Header("Lid Settings")] 
	public Transform lidObject;
	public float openAngle = -45f;
	public float duration = 1f;
	
	[Header("VFX")]
	[SerializeField] private GameObject vfxChestActive;
	[SerializeField] private GameObject vfxChestStatic;

	private bool isOpen = false;

	private void OnEnable()
	{
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OnInventoryClosed +=
			HandleInventoryClosed;
	}

	private void OnDisable()
	{
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OnInventoryClosed -=
			HandleInventoryClosed;
	}

	public override void Interact()
	{
		base.Interact();

		PlayerInventorySystem.Instance.CurrentExternalReceiver = this;
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenPanelByName("Chest");
		ToggleLid();
		tutorialUIGroup?.Show();
	}

	private void HandleInventoryClosed()
	{
		if (isOpen)
			ToggleLid();
	}

	public void ToggleLid()
	{
		float targetAngle = isOpen ? 0f : openAngle;

		if (lidObject != null)
			lidObject.DOLocalRotate(new Vector3(targetAngle, 0f, 0f), duration, RotateMode.Fast);
		
		isOpen = !isOpen;

		if (vfxChestActive != null)
			vfxChestActive.SetActive(isOpen);

		if (vfxChestStatic != null)
			vfxChestStatic.SetActive(!isOpen);
		
	}
	
	public void ClearSlots()
	{
		for (int i = 0; i < slots.Count; i++)
		{
			slots[i].ItemData = null;
			slots[i].Count = 0;
		}
	}

}