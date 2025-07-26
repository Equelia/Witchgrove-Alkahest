using TMPro;
using UnityEngine;

public class GoalUI : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI goalText;

	public void SetGoalText(string goal)
	{
		// плавная анимация появления текста с помощью DoTween
		goalText.text = $"Цель: " + $"" + $"{goal}";
	}

	public void FinishAllGoals()
	{
		// плавная анимация исчезновения текста с помощью DoTween
		gameObject.SetActive(false);
	}
}