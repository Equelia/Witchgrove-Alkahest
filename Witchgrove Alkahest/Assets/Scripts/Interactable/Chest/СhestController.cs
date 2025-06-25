using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChestSave
{
	public string chestId;
	public List<SaveData.SlotData> slots;
}

public class СhestController : MonoBehaviour
{
	public List<Chest> chests;

	private void Awake()
	{
		for (int i = 0; i < chests.Count; i++)
		{
			chests[i].ChestId = i.ToString();
		}
	}

	public Chest GetChestById(string id)
	{
		for (int i = 0; i < chests.Count; i++)
		{
			if (chests[i].ChestId == id)
				return chests[i];
				
		}
		
		Debug.LogError("Chest with this id not found");
		return null;
	}
}
