using TMPro;
using UnityEngine;
using DG.Tweening;

public class GoalUI : MonoBehaviour
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private TextMeshProUGUI goalText;
	[SerializeField] private RectTransform rectTransform;

	[SerializeField] private float showDuration = 0.8f;
	[SerializeField] private float hideDuration = 0.7f;
	[SerializeField] private float bounceOffset = 60f;
	[SerializeField] private float flyOffset = 150f;

	private Vector2 originalPosition;

	private void Awake()
	{
		originalPosition = rectTransform.anchoredPosition;
	}

	public void SetGoalText(string goal)
	{
		if (string.IsNullOrEmpty(goal))
			goalText.text = "";
		else
			goalText.text = $"Цель: {goal}";

		gameObject.SetActive(true);

		canvasGroup.DOKill();
		rectTransform.DOKill();

		canvasGroup.alpha = 0f;
		rectTransform.anchoredPosition = originalPosition + Vector2.up * bounceOffset;

		Sequence seq = DOTween.Sequence();
		seq.Join(canvasGroup.DOFade(1f, showDuration).SetEase(Ease.OutQuad));
		seq.Join(rectTransform.DOAnchorPos(originalPosition, showDuration).SetEase(Ease.OutBounce));
	}

	public void HideGoals()
	{
		canvasGroup.DOKill();
		rectTransform.DOKill();

		Sequence seq = DOTween.Sequence();
		seq.Join(canvasGroup.DOFade(0f, hideDuration).SetEase(Ease.InQuad));
		seq.Join(rectTransform.DOAnchorPos(originalPosition + Vector2.up * flyOffset, hideDuration).SetEase(Ease.InBack));
		seq.OnComplete(() =>
		{
			gameObject.SetActive(false);
			rectTransform.anchoredPosition = originalPosition;
		});
	}

	public void HideInstantly()
	{
		canvasGroup.DOKill();
		rectTransform.DOKill();

		gameObject.SetActive(false);
		canvasGroup.alpha = 0f;
		rectTransform.anchoredPosition = originalPosition;
	}
}