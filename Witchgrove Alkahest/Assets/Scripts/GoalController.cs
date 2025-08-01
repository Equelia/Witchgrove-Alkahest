using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class GoalStep
{
	public GoalData data;
	public GoalConditionType conditionType;
}

public enum GoalConditionType
{
	InitialGoal,
	OpenQuestBoard,
	PinRecipe,
	CraftPotionFromPool,
	EnterBiome,
	QuestAvailable,
	UseAltar
}


public class GoalController : MonoBehaviour
{
	public static GoalController Instance { get; private set; }

	[SerializeField] private GameObject goalCat;
	[SerializeField] private Transform playerTransform;
	[SerializeField] private List<GoalStep> goalSteps;

	[SerializeField] private GoalUI goalUI;

	private int currentStepIndex;

	public GoalStep CurrentStep => goalSteps[currentStepIndex];

	private void Awake()
	{
		if (Instance == null) Instance = this;
		else Destroy(gameObject);
	}

	private void Start()
	{
		if (currentStepIndex == 0)
			SpawnGoalCat(true);
	}


	public void TriggerGoalProgress(GoalConditionType conditionType)
	{
		var step = CurrentStep;

		if (step.conditionType == conditionType)
		{
			SpawnGoalCat();
			goalUI.FinishAllGoals();
		}
	}

	private void SpawnGoalCat(bool spawnForward = false)
	{
		Vector3 spawnPos = playerTransform.position - playerTransform.forward * 2f;

		if (spawnForward)
			spawnPos = playerTransform.position + playerTransform.forward * 2f;

		goalCat.transform.position = spawnPos;
		goalCat.SetActive(true);
	}

	public void AdvanceGoal()
	{
		currentStepIndex++;
		if (currentStepIndex < goalSteps.Count)
		{
			goalUI.SetGoalText(CurrentStep.data.description);
		}
		else
		{
			goalUI.FinishAllGoals();
		}
	}

	public int GetCurrentStepIndex() => currentStepIndex;

	public void SetCurrentStepIndex(int index)
	{
		currentStepIndex = Mathf.Clamp(index, 0, goalSteps.Count - 1);
		goalUI.SetGoalText(CurrentStep.data.description);
	}
}