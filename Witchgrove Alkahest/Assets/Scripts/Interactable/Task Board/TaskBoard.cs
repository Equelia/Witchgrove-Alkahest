using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskBoard : InteractableItem
{
	[Header("Basket & PlayerExperience Components")]
	[SerializeField] private Basket basket;
	[SerializeField] private PlayerExperience playerExperience;
	[SerializeField] private PlayerData playerData;
	
	[Header("Tutorial")]
	[SerializeField] private TutorialUIGroup tutorialUIGroup;
	
	[Header("Quest's Data")]
	[Tooltip("Quest Databases by Level Index (e.g. 0 → 1-lvl quests, 1 → 2-lvl, etc.)")]
	[SerializeField] private QuestDatabase[] questDatabases;
	
	[HideInInspector] public QuestData activeQuest;
	private List<QuestData> completedQuest = new();
	private List<QuestData> allQuests = new();

	private void Awake()
	{
		activeQuest = null;
		allQuests = questDatabases
			.Where(qd => qd != null)
			.SelectMany(qd => qd.quests)
			.ToList();
	}

	public override void Interact()
	{
		base.Interact();
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenPanelByName("TaskBoard");
		tutorialUIGroup?.Show();
	}

	public List<QuestData> GetAvailableQuests()
	{
		var available = new List<QuestData>();

		foreach (var db in questDatabases)
		{
			if (db == null) continue;
			if (playerData.Level < db.requiredLevel) continue;

			var levelQuests = db.quests
				.Where(q => !completedQuest.Contains(q));
			available.AddRange(levelQuests);
		}

		return available;
	}

	
	public List<QuestData> GetCompletedQuests() => completedQuest;
	
	public void SetCompletedQuestsByIds(List<string> ids)
	{
		completedQuest.Clear();

		foreach (var qd in questDatabases)
		{
			if (qd == null) continue;

			foreach (var q in qd.quests)
			{
				if (ids.Contains(q.questId))
					completedQuest.Add(q);
			}
		}
	}
	
	public void MarkQuestCompleted()
	{
		if (!completedQuest.Contains(activeQuest))
			completedQuest.Add(activeQuest);

		ConsumeItems();
		playerExperience.AddExp(activeQuest.expAmount);
		Debug.Log($"Задание \"{activeQuest.questId}\" выполнено! Начисленно \"{activeQuest.expAmount}\" опыта!");
		activeQuest = null;
	}

	public int GetBasketAvailableItems(QuestData quest)
	{
		if (quest == null) return 0;

		return basket.GetAllSlots()
			.Where(slot => slot.ItemData == quest.requiredItem)
			.Sum(slot => slot.Count);
	}

	private void ConsumeItems()
	{
		int remaining = activeQuest.requiredCount;
		
		foreach (var slot in basket.GetAllSlots())
		{
			if (slot.ItemData != activeQuest.requiredItem) continue;

			int used = Mathf.Min(remaining, slot.Count);
			slot.Count -= used;
			remaining -= used;

			if (slot.Count <= 0)
				slot.ItemData = null;

			if (remaining <= 0)
				break;
		}
	}
}
