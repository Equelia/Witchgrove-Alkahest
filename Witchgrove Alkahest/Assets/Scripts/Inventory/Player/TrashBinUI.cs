using UnityEngine;
using UnityEngine.UI;

public class TrashBinUI : MonoBehaviour
{
	[SerializeField] private Image trashSlotRadialTimer;
	[SerializeField] private CellController[] trashBinCells;

	private void Start()
	{
		var trashSlots = PlayerInventorySystem.Instance.trashBinSlots;

		for (int i = 0; i < trashBinCells.Length && i < trashSlots.Count; i++)
			trashBinCells[i].Setup(trashSlots[i], trashSlots, i);

		foreach (var slot in trashSlots)
			slot.OnSlotChanged += HandleTrashBinSlotChanged;
	}

	private void HandleTrashBinSlotChanged(Cell slot)
	{
		bool has = slot.Count > 0 && slot.ItemData != null;

		if (has)
		{
			PlayerInventorySystem.Instance.StartTrashTimer();
			trashSlotRadialTimer.gameObject.SetActive(true);
		}
		else
		{
			PlayerInventorySystem.Instance.CancelTrashTimer();
			trashSlotRadialTimer.gameObject.SetActive(false);
		}
	}

	private void Update()
	{
		trashSlotRadialTimer.fillAmount = PlayerInventorySystem.Instance.GetTrashProgress();
	}
}