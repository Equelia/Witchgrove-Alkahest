using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class PlayerExperience : MonoBehaviour
{
	[Header("EXP Thresholds (cumulative)")] [SerializeField]
	private List<float> expThresholds = new List<float> { 0, 10, 30, 50, 90, 130, 250, 370 };

	[SerializeField] private PlayerData playerData;

	public void AddExp(float amount)
	{
		playerData.TotalExp += amount;

		int previousLevel = playerData.Level;

		while (playerData.Level < expThresholds.Count && playerData.TotalExp >= expThresholds[playerData.Level])
		{
			playerData.Level++;
		}

		if (playerData.Level != previousLevel)
		{
			playerData.InvokeLevelChanged();
		}

		SaveManager.Instance.SaveGame();
	}



	public float GetProgressToNextLevel()
	{
		if (playerData.Level >= expThresholds.Count) return 1f;
		if (playerData.Level <= 0) return 0f;

		float prev = expThresholds[playerData.Level - 1];
		float next = expThresholds[playerData.Level];
		return (playerData.TotalExp - prev) / (next - prev);
	}
}