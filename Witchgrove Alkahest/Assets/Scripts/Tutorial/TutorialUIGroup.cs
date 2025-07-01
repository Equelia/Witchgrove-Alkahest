using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class TutorialUIGroup : MonoBehaviour
{
	[Tooltip("Уникальный ID этого окна, чтобы показывать только один раз")]
	public string tutorialId;

	[Header("DOTween Settings")]
	[SerializeField] private float fadeDuration = 0.4f;
	[SerializeField] private float scaleDuration = 0.4f;
	[SerializeField] private float displayTime = 3f;

	private CanvasGroup canvasGroup;
	private Transform uiTransform;
	private Sequence showSequence;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		uiTransform = transform;

		canvasGroup.alpha = 0f;
		uiTransform.localScale = Vector3.zero;
		gameObject.SetActive(false);
	}

	private void Start()
	{
		if (!TutorialManager.HasSeen(tutorialId))
		{
			Show();
		}
	}

	public void Show()
	{
		if (TutorialManager.HasSeen(tutorialId)) return;

		TutorialManager.MarkAsSeen(tutorialId);
		gameObject.SetActive(true);

		showSequence?.Kill();

		showSequence = DOTween.Sequence();

		showSequence
			.Append(canvasGroup.DOFade(1f, fadeDuration))
			.Join(uiTransform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack))
			.AppendInterval(displayTime)
			.Append(canvasGroup.DOFade(0f, fadeDuration))
			.Join(uiTransform.DOScale(Vector3.zero, scaleDuration).SetEase(Ease.InBack))
			.OnComplete(() => gameObject.SetActive(false));
	}

	public void Hide()
	{
		showSequence?.Kill();

		// Instantly hide
		canvasGroup.DOFade(0f, fadeDuration);
		uiTransform.DOScale(Vector3.zero, scaleDuration).OnComplete(() => gameObject.SetActive(false));
	}
}