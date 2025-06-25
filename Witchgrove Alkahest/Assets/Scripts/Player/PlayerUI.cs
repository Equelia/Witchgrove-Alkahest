using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerUI : MonoBehaviour
{
	[SerializeField] private PlayerData playerData;
	[SerializeField] private TMP_Text levelText;
	[SerializeField] private TMP_Text goldText;

	private void Start()
	{
		UpdateLevelUI();
		UpdateGoldUI();

		playerData.OnLevelChanged += AnimateLevelUp;
		playerData.OnGoldChanged += AnimateGoldChange;
	}

	private void OnDisable()
	{
		playerData.OnLevelChanged -= AnimateLevelUp;
		playerData.OnGoldChanged -= AnimateGoldChange;
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

	private int currentGoldDisplay = 0;

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

		// Анимация роста числа
		DOTween.To(() => currentGoldDisplay, x =>
		{
			currentGoldDisplay = x;
			goldText.text = x.ToString();
		}, playerData.GoldAmount, 0.5f).SetEase(Ease.OutQuad);
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