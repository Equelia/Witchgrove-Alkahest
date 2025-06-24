using System;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
	private int _level = 1;
	private float _totalExp;
	private int goldAmountAmount;
	
	public event Action OnLevelChanged;
	public event Action OnExpChanged;
	public event Action OnGoldChanged;

	public int Level
	{
		get => _level;
		set
		{
			_level = value;
			OnLevelChanged?.Invoke();
		}
	}

	public float TotalExp
	{
		get => _totalExp;
		set
		{
			_totalExp = value;
			OnExpChanged?.Invoke();
		}
	}

	public int GoldAmount
	{
		get => goldAmountAmount;
		set
		{
			goldAmountAmount = value;
			OnGoldChanged?.Invoke();
		}
	}
}
