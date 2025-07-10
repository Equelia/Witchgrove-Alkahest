using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class UIWindowGroup : MonoBehaviour
{
	[Tooltip("Uniqe Id of the Tutorial UI Group")]
	public string tutorialId;

	[Header("DOTween Settings")]
	[SerializeField] private float fadeDuration = 0.4f;
	[SerializeField] private float scaleDuration = 0.4f;

	[Header("UI References")]
	[SerializeField] private Button closeButton;
	[SerializeField] private CanvasGroup holderGroup;
	[SerializeField] private Transform holderTransform;

	[Header("Setting")] 
	[SerializeField] private bool showOnce = true;

	private CanvasGroup canvasGroup;
	private Sequence showSequence;

	private void Awake()
	{
		canvasGroup = GetComponent<CanvasGroup>();
		
		closeButton.onClick.AddListener(Hide);

		canvasGroup.alpha = 0f;
		
		if(holderGroup != null)
			holderGroup.alpha = 0f;
		
		if(holderTransform != null)
			holderTransform.localScale = Vector3.zero;
		
		gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		closeButton.onClick.RemoveListener(Hide);
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
		if (showOnce)
		{
			if (TutorialManager.HasSeen(tutorialId)) 
				return;
			
			TutorialManager.MarkAsSeen(tutorialId);
		}
		
		gameObject.SetActive(true);

		showSequence?.Kill();
		showSequence = DOTween.Sequence();

		showSequence
			.Append(canvasGroup.DOFade(1f, fadeDuration))
			.Join(transform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutQuad))
			.Append(holderGroup.DOFade(1f, fadeDuration))
			.Join(holderTransform.DOScale(Vector3.one, scaleDuration).SetEase(Ease.OutBack));
	}

	public void Hide()
	{
		showSequence?.Kill();

		Sequence hideSequence = DOTween.Sequence();

		hideSequence
			.Append(holderGroup.DOFade(0f, fadeDuration))
			.Join(holderTransform.DOScale(Vector3.zero, scaleDuration))
			.Append(canvasGroup.DOFade(0f, fadeDuration))
			.OnComplete(() => gameObject.SetActive(false));
	}
}