using TMPro;
using UnityEngine;
using DG.Tweening;

public class PlayerLevelUI : MonoBehaviour
{
	[SerializeField] private PlayerData playerData;
	[SerializeField] private TMP_Text levelText;

	private void OnEnable()
	{
		UpdateLevelUI();
		playerData.OnLevelChanged += AnimateLevelUp;
	}

	private void OnDisable()
	{
		playerData.OnLevelChanged -= AnimateLevelUp;
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

	private void UpdateLevelUI()
	{
		levelText.text = playerData.Level.ToString();
	}
}