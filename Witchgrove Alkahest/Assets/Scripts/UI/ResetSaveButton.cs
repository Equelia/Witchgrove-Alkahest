using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class ResetSaveButton : MonoBehaviour
{
	private Button button;
	private string savePath;

	private void Awake()
	{
		button = GetComponent<Button>();
		savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
	}

	private void OnEnable()
	{
		button.onClick.AddListener(ResetSaveData);
	}

	private void OnDisable()
	{
		button.onClick.RemoveListener(ResetSaveData);
	}

	private void ResetSaveData()
	{
		DeleteOnlyGamePlayerPrefs();
		ResetSaveFile();

		if (Application.isPlaying && SaveManager.Instance != null)
		{
			SaveManager.Instance.LoadGame();
		}

		Debug.Log("[ResetSaveButton] Игра обнулена. Начало новой игры.");
	}

	private void DeleteOnlyGamePlayerPrefs()
	{
		// Сохраняем настройки
		var savedSettings = new Dictionary<string, object>
		{
			{ "MouseSensitivity", PlayerPrefs.GetFloat("MouseSensitivity", 0.5f) },
			{ "FpsLimit", PlayerPrefs.GetInt("FpsLimit", 0) },
			{ "ScreenResolution", PlayerPrefs.GetInt("ScreenResolution", 0) },
			{ "QualityLevel", PlayerPrefs.GetInt("QualityLevel", 2) },
			{ "BrightnessLevel", PlayerPrefs.GetFloat("BrightnessLevel", 0.3f) },
			{ "MasterVolume", PlayerPrefs.GetFloat("MasterVolume", 0.7f) },
			{ "MusicVolume", PlayerPrefs.GetFloat("MusicVolume", 0.2f) },
			{ "SfxVolume", PlayerPrefs.GetFloat("SfxVolume", 0.5f) }
		};

		// Удаляем все PlayerPrefs
		PlayerPrefs.DeleteAll();

		// Восстанавливаем настройки
		foreach (var pair in savedSettings)
		{
			switch (pair.Value)
			{
				case int i:
					PlayerPrefs.SetInt(pair.Key, i);
					break;
				case float f:
					PlayerPrefs.SetFloat(pair.Key, f);
					break;
				case string s:
					PlayerPrefs.SetString(pair.Key, s);
					break;
			}
		}

		PlayerPrefs.Save();
	}

	private void ResetSaveFile()
	{
		if (File.Exists(savePath))
			File.Delete(savePath);

		SaveData newSave = new SaveData
		{
			playerLevel = 1,
			playerExp = 0f,
			playerGold = 0,
			playerInventoryLevel = 1,
			playerInventory = new List<SaveData.SlotData>(),
			cauldronCraftSlots = new List<SaveData.SlotData>(),
			basketSlots = new List<SaveData.SlotData>(),
			chests = new List<ChestSave>(),
			completedQuestIds = new List<string>()
		};

		string json = JsonUtility.ToJson(newSave, true);
		File.WriteAllText(savePath, json);
	}
}
