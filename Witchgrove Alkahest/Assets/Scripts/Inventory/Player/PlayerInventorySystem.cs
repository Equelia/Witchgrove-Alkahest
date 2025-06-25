using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

/// <summary>
/// Inventory system specifically for the player, built on top of InventoryProvider.
/// Supports external receivers, trash logic, and UI linkage.
/// </summary>
public class PlayerInventorySystem : InventoryProvider
{
	public static PlayerInventorySystem Instance { get; private set; }

	[Header("Player Inventory UI")] public PlayerInventoryUI playerInventoryUI;
	
	[Header("InventorySlots")]
	[SerializeField] private GameObject[] inventoryRows; 
	public int maxInventoryLevel = 3;
	
	public PlayerData playerData;

	public IExternalInventoryReceiver CurrentExternalReceiver;

	[HideInInspector] public List<Cell> trashBinSlots;
	private readonly Dictionary<Cell, CancellationTokenSource> _trashCts = new();

	private float trashStartTime = -1f;

	public override void Awake()
	{
		base.Awake(); // initializes "slots" from InventoryProvider

		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}

		Instance = this;
		DontDestroyOnLoad(gameObject);

		// Set up trash bin
		trashBinSlots = new List<Cell>(1);
		var trashSlot = new Cell();
		trashSlot.OnSlotChanged += HandleTrashSlotChanged;
		trashBinSlots.Add(trashSlot);
	}
	
	public void AddToFirstEmpty(BaseItemData item, int count)
	{
		for (int i = 0; i < count; i++)
			TryAddOneItem(item);
	}

	//For future use
	public bool TryConsumeItem(BaseItemData item, int amount)
	{
		foreach (var slot in slots)
		{
			if (slot.ItemData == item && slot.Count >= amount)
			{
				slot.Count -= amount;
				if (slot.Count == 0)
					slot.ItemData = null;
				return true;
			}
		}

		return false;
	}
	
	
	public void UpgradeInventory()
	{
		if (playerData.InventoryLevel >= maxInventoryLevel)
			return;

		playerData.InventoryLevel++;
		ApplyInventorySize();
		SaveManager.Instance.SaveGame();
	}
	
	public void ApplyInventorySize()
	{
		// Включаем нужные строки UI
		for (int i = 0; i < inventoryRows.Length; i++)
			inventoryRows[i].SetActive(i < playerData.InventoryLevel);

		// Обновляем количество слотов (точно)
		int requiredSlotCount = GetUnlockedSlotCount();

		// Если слотов меньше — добавляем
		while (slots.Count < requiredSlotCount)
		{
			slots.Add(new Cell());
		}

		// Если слотов больше — обрезаем (на случай отладки)
		if (slots.Count > requiredSlotCount)
		{
			slots.RemoveRange(requiredSlotCount, slots.Count - requiredSlotCount);
		}

		playerInventoryUI.RefreshUI();
	}


	
	public int GetUnlockedSlotCount()
	{
		return playerData.InventoryLevel * 4;
	}

	#region TrashBin

	private void HandleTrashSlotChanged(Cell slot)
	{
		if (slot.Count > 0 && slot.ItemData != null)
		{
			CancelTrashDeletion(slot);
			var cts = new CancellationTokenSource();
			_trashCts[slot] = cts;
			ClearTrashSlotAfterDelay(slot, cts.Token).Forget();
		}
		else
		{
			CancelTrashDeletion(slot);
		}
	}

	private void CancelTrashDeletion(Cell slot)
	{
		if (_trashCts.TryGetValue(slot, out var cts))
		{
			cts.Cancel();
			cts.Dispose();
			_trashCts.Remove(slot);
		}
	}

	private async UniTaskVoid ClearTrashSlotAfterDelay(Cell slot, CancellationToken ct)
	{
		try
		{
			await UniTask.Delay(TimeSpan.FromSeconds(3), cancellationToken: ct);
			slot.Count = 0;
			slot.ItemData = null;
			DragManager.Instance.EndDrag();
		}
		catch (OperationCanceledException)
		{
			// Canceled deletion
		}
	}

	public void StartTrashTimer() => trashStartTime = Time.time;

	public void CancelTrashTimer() => trashStartTime = -1f;

	public float GetTrashProgress()
	{
		if (trashStartTime < 0f) return 0f;
		return Mathf.Clamp01((Time.time - trashStartTime) / 3f);
	}

	#endregion
}