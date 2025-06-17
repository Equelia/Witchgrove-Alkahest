using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TaskBoardUI : MonoBehaviour
{
	[Header("Task Board Components")] [Tooltip("Assign Task Board logic components")] [SerializeField]
	private TaskBoard taskBoardController;

	[Header("Task Board QuestButton Elements"), Space(10f)] [SerializeField]
	private List<Button> questButtons;

	[Header("UI Elements"), Space(10f)] [Tooltip("Assign UI Elements")] [SerializeField]
	private TMP_Text infoText;

	[SerializeField] private TMP_Text potionCountText;
	[SerializeField] private Button completeButton;

	private void OnEnable()
	{
		completeButton.onClick.AddListener(CompleteQuest);
	}

	private void OnDisable()
	{
		completeButton.onClick.RemoveListener(CompleteQuest);
	}

	private void Start()
	{
		UpdateQuestStatus();
		RedrawQuestList();
		completeButton.interactable = false;
	}

	private void RedrawQuestList()
	{
		var availableQuest = taskBoardController.GetAvailableQuests();

		for (int i = 0; i < questButtons.Count; i++)
		{
			var button = questButtons[i];

			if (i < availableQuest.Count)
			{
				var quest = availableQuest[i];

				button.gameObject.SetActive(true);
				button.GetComponentInChildren<TMP_Text>().text = availableQuest[i].questId;

				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(() => SelectQuest(quest));
			}
			else
			{
				button.gameObject.SetActive(false);
			}
		}
	}

	private void SelectQuest(QuestData quest)
	{
		taskBoardController.activeQuest = quest;
		UpdateQuestStatus(quest);
	}

	private void UpdateQuestStatus(QuestData activeQuest = null)
	{
		if (taskBoardController.activeQuest == null)
		{
			potionCountText.text = "Зелий в наличии: 0/0";
			infoText.text = "";
			completeButton.interactable = false;

			return;
		}

		UpdateAvailableItemsCount();
		infoText.text = activeQuest.description;
	}

	public void UpdateAvailableItemsCount()
	{
		var activeQuest = taskBoardController.activeQuest;
		int count = taskBoardController.GetBasketAvailableItems(activeQuest);

		if (activeQuest != null)
		{
			completeButton.interactable = count >= activeQuest.requiredCount;
			potionCountText.text = $"Зелий в наличии: {count}/{activeQuest.requiredCount}";

		}
		else
		{
			potionCountText.text = "Зелий в наличии: 0/0";
			completeButton.interactable = false;
		}

	}

	public void CompleteQuest()
	{
		if (taskBoardController.activeQuest == null) return;

		taskBoardController.MarkQuestCompleted();
		RedrawQuestList();
		UpdateQuestStatus();
	}
}