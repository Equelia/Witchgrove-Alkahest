using TMPro;
using UnityEngine;
using DG.Tweening;

public class GoalUI : MonoBehaviour
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private TextMeshProUGUI goalText;

	private void Awake()
	{
		canvasGroup.alpha = 0f;
	}

	public void SetGoalText(string goal)
	{
		if (string.IsNullOrEmpty(goal))
			goalText.text = "";
		else
			goalText.text = $"Цель: {goal}";
		gameObject.SetActive(true);

		canvasGroup.DOKill();

		canvasGroup.DOFade(1f, 1f).SetEase(Ease.OutQuad);
	}

	public void FinishAllGoals()
	{
		canvasGroup.DOFade(0f, 1f)
			.SetEase(Ease.InQuad)
			.OnComplete(() => gameObject.SetActive(false));
	}
}