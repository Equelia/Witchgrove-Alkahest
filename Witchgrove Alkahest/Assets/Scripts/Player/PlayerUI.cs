using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
	[Header("Player Data")]
	[SerializeField] private PlayerData playerData;
	[SerializeField] private PlayerExperience playerExperience;
	
	[Header("UI Elements")]
	[SerializeField] private TMP_Text levelText;
	[SerializeField] private TMP_Text goldText;
	[SerializeField] private Image expProgressBar; 
	
	private int currentGoldDisplay = 0;

	private void Start()
	{
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

	private void AnimateLevelUp()
	{
		levelText.text = playerData.Level.ToString();

		levelText.transform.DOKill();
		levelText.transform.localScale = Vector3.one;

		levelText.transform
			.DOScale(1.4f, 0.2f)
			.SetEase(Ease.OutBack)
			.OnComplete(() =>
				levelText.transform.DOScale(1f, 0.2f).SetEase(Ease.InBack)
			);

		levelText.DOColor(Color.yellow, 0.15f)
			.OnComplete(() =>
				levelText.DOColor(Color.white, 0.15f)
			);
	}

	
	private void AnimateGoldChange()
	{
		goldText.transform.DOKill();
		goldText.transform.localScale = Vector3.one;

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
		float currentProgress = expProgressBar.fillAmount;
		float targetProgress = playerExperience.GetProgressToNextLevel();

		if (targetProgress < 1f)
		{
			expProgressBar.DOFillAmount(targetProgress, 0.5f).SetEase(Ease.OutQuad);
		}
		else
		{
			var sequence = DOTween.Sequence();

			sequence.Append(expProgressBar.DOFillAmount(1f, 0.4f).SetEase(Ease.OutQuad));
			sequence.AppendInterval(0.1f);
			sequence.Append(expProgressBar.DOFillAmount(0f, 0.3f).SetEase(Ease.InQuad));
			sequence.AppendCallback(() =>
			{
				float newProgress = playerExperience.GetProgressToNextLevel();
				expProgressBar.DOFillAmount(newProgress, 0.4f).SetEase(Ease.OutQuad);
			});
		}
	}

	
	private void UpdateExpProgressBar()
	{
		float currentProgress = playerExperience.GetProgressToNextLevel();
		expProgressBar.fillAmount = currentProgress;
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