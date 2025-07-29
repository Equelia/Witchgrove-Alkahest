using System;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("Player Data")] [SerializeField]
	private PlayerData playerData;

	[SerializeField] private PlayerExperience playerExperience;

	[Header("UI Elements")] [SerializeField]
	private TMP_Text levelText;

	[SerializeField] private TMP_Text goldText;
	[SerializeField] private Image expProgressBar;
	[SerializeField] private RectTransform levelEffectTransform;

	private int currentGoldDisplay = 0;

	private void OnEnable()
	{
		if (playerData == null || playerExperience == null)
		{
			Debug.LogWarning("PlayerUI: missing references");
			return;
		}

		UpdateLevelUI();
		UpdateGoldUI();
		UpdateExpProgressBar();

		playerData.OnLevelChanged += AnimateLevelUp;
		playerData.OnGoldChanged += AnimateGoldChange;
		playerData.OnExpChanged += AnimateExpProgress;
	}

	private void OnDisable()
	{
		playerData.OnLevelChanged -= AnimateLevelUp;
		playerData.OnGoldChanged -= AnimateGoldChange;
		playerData.OnExpChanged -= AnimateExpProgress;
	}

	private void Update()
	{
		if (expProgressBar.fillAmount == 0)
			levelEffectTransform.gameObject.SetActive(false);
		else
			levelEffectTransform.gameObject.SetActive(true);
	}

	private void AnimateLevelUp()
	{
		SoundManager.Instance.PlaySound("LevelUp");

		levelText.text = playerData.Level.ToString();

		levelText.transform.DOKill();
		levelText.transform.localScale = Vector3.one;

		levelText.transform
			.DOScale(1.4f, 0.2f)
			.SetEase(Ease.OutBack)
			.OnComplete(() =>
				levelText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack)
			);
	}


	private void AnimateGoldChange()
	{
		goldText.transform.DOKill();
		goldText.transform.localScale = Vector3.one;

		SoundManager.Instance.PlaySound("GoldChange");

		goldText.transform
			.DOScale(1.3f, 0.2f)
			.SetEase(Ease.OutBack)
			.OnComplete(() =>
				goldText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack)
			);

		goldText.DOColor(Color.yellow, 0.15f)
			.OnComplete(() =>
				goldText.DOColor(Color.white, 0.15f)
			);

		DOTween.To(() => currentGoldDisplay, x =>
		{
			currentGoldDisplay = x;
			goldText.text = x.ToString();
		}, playerData.GoldAmount, 0.5f).SetEase(Ease.OutQuad);
	}

	private void AnimateExpProgress()
	{
		float targetProgress = playerExperience.GetProgressToNextLevel();

		if (targetProgress < 1f)
		{
			float currentProgress = expProgressBar.fillAmount;

			DOTween.To(() => currentProgress, x =>
			{
				expProgressBar.fillAmount = x;
				UpdateLevelEffectVisual(x);
			}, targetProgress, 0.5f).SetEase(Ease.OutQuad);
		}
		else
		{
			var sequence = DOTween.Sequence();

			// До 100%
			sequence.Append(DOTween.To(() => expProgressBar.fillAmount, x =>
			{
				expProgressBar.fillAmount = x;
				UpdateLevelEffectVisual(x);
			}, 1f, 0.4f).SetEase(Ease.OutQuad));

			sequence.AppendInterval(0.1f);

			sequence.AppendCallback(() => { UpdateLevelEffectVisual(1f); });

			sequence.Append(DOTween.To(() => 1f, x =>
			{
				expProgressBar.fillAmount = x;
				UpdateLevelEffectVisual(x);
			}, 0f, 0.3f).SetEase(Ease.InQuad));

			sequence.AppendCallback(() =>
			{
				float newProgress = playerExperience.GetProgressToNextLevel();

				DOTween.To(() => 0f, x =>
				{
					expProgressBar.fillAmount = x;
					UpdateLevelEffectVisual(x);
				}, newProgress, 0.4f).SetEase(Ease.OutQuad);
			});
		}
	}


	private void UpdateLevelEffectVisual(float progress)
	{
		RectTransform effectRect = levelEffectTransform;
		RectTransform parentRect = expProgressBar.rectTransform;

		float totalHeight = parentRect.rect.height;

		const float yOffset = -5f;
		float targetY = progress * totalHeight + yOffset;

		effectRect.anchoredPosition = new Vector2(
			effectRect.anchoredPosition.x,
			targetY
		);
	}


	private void UpdateExpProgressBar()
	{
		float currentProgress = playerExperience.GetProgressToNextLevel();
		expProgressBar.fillAmount = currentProgress;
		UpdateLevelEffectVisual(currentProgress);
	}


	private void UpdateLevelUI()
	{
		levelText.text = playerData.Level.ToString();
	}

	private void UpdateGoldUI()
	{
		currentGoldDisplay = playerData.GoldAmount;
		goldText.text = currentGoldDisplay.ToString();
	}
}