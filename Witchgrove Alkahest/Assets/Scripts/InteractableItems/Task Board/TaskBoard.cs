using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskBoard : InteractableItem
{
	[Header("Basket Component")]
	[SerializeField] private Basket basket;
	
	[Header("Quest's Data")]
	[Tooltip("Assign current location quest list")]
	[SerializeField] private QuestDatabase questDatabase;
	
	[HideInInspector] public QuestData activeQuest;
	private List<QuestData> completedQuest = new();
	private List<QuestData> allQuests = new();

	private void Awake()
	{
		activeQuest = null;
		allQuests = questDatabase.quests;
	}

	public override void Interact()
	{
		base.Interact();
		InventorySystem.Instance.inventoryUI.OpenPanelByName("TaskBoard");
	}

	public List<QuestData> GetAvailableQuests()
	{
		List<QuestData> availableQuests = allQuests
			.Where(q => !completedQuest.Contains(q))
			.ToList();
		
		return availableQuests;
	}
	
	public void MarkQuestCompleted()
	{
		if (!completedQuest.Contains(activeQuest))
			completedQuest.Add(activeQuest);

		ConsumeItems();
		Debug.Log($"Задание \"{activeQuest.questId}\" выполнено!");
		activeQuest = null;
	}

	public int GetBasketAvailableItems(QuestData quest)
	{
		int count = 0;

		if (quest != null)
		{
			foreach (var slot in basket.basketCells)
			{
				if (slot.ItemData == quest.requiredItem)
					count += slot.Count;
			}
		}
		
		
		return count;
	}

	private void ConsumeItems()
	{
		int remaining = activeQuest.requiredCount;
		
		foreach (var slot in basket.basketCells)
		{
			if (slot.ItemData == activeQuest.requiredItem)
			{
				int used = Mathf.Min(remaining, slot.Count);
				slot.Count -= used;
				remaining -= used;

				if (slot.Count <= 0)
					slot.ItemData = null;

				if (remaining <= 0)
					break;
			}
		}
		
		basket.basketUI.RefreshCellsUI();
	}
}
