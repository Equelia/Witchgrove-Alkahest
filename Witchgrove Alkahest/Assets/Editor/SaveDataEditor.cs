#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System.IO;

/// <summary>
/// SaveData manager
/// </summary>
public class SaveDataEditor : OdinEditorWindow
{
	[MenuItem("Tools/Save Data Editor")]
	private static void OpenWindow()
	{
		GetWindow<SaveDataEditor>().Show();
	}

	private SaveData currentSaveData;

	[BoxGroup("Player Data"), LabelText("Уровень игрока"), ShowInInspector]
	public int playerLevel;

	[BoxGroup("Player Data"), LabelText("Опыт игрока"), ShowInInspector]
	public float playerExp;

	[BoxGroup("Player Data"), LabelText("Золото"), ShowInInspector]
	public int playerGold;

	[BoxGroup("Player Data"), LabelText("Уровень инвентаря"), ShowInInspector]
	public int inventoryLevel;

	private string savePath;

	protected override void OnEnable()
	{
		base.OnEnable();
		savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
		LoadSaveData();
	}

	[Button(ButtonSizes.Large), GUIColor(0.6f, 0.8f, 1f)]
	private void LoadSaveData()
	{
		if (File.Exists(savePath))
		{
			var json = File.ReadAllText(savePath);
			currentSaveData = JsonUtility.FromJson<SaveData>(json);
		}
		else
		{
			currentSaveData = new SaveData();
		}

		playerLevel = currentSaveData.playerLevel;
		playerExp = currentSaveData.playerExp;
		playerGold = currentSaveData.playerGold;
		inventoryLevel = currentSaveData.playerInventoryLevel;

		Debug.Log("[SaveDataEditor] Данные загружены.");
	}

	[Button(ButtonSizes.Large), GUIColor(1f, 0.8f, 0.4f)]
	private void ApplyChanges()
	{
		currentSaveData.playerLevel = playerLevel;
		currentSaveData.playerExp = playerExp;
		currentSaveData.playerGold = playerGold;
		currentSaveData.playerInventoryLevel = inventoryLevel;

		var json = JsonUtility.ToJson(currentSaveData, true);
		File.WriteAllText(savePath, json);

#if UNITY_EDITOR
		if (Application.isPlaying && SaveManager.Instance != null)
		{
			SaveManager.Instance.LoadGame();
			Debug.Log("[SaveDataEditor] Данные применены и загружены в рантайме.");
		}
		else
		{
			Debug.Log("[SaveDataEditor] Данные сохранены.");
		}
#endif
	}

	[Button(ButtonSizes.Medium), GUIColor(0.7f, 1f, 0.7f)]
	private void ReloadFromRuntime()
	{
		if (Application.isPlaying && SaveManager.Instance != null)
		{
			SaveManager.Instance.SaveGame();
			LoadSaveData();
			Debug.Log("[SaveDataEditor] Получены данные из рантайма.");
		}
		else
		{
			Debug.LogWarning("[SaveDataEditor] Игра не запущена. Невозможно получить runtime данные.");
		}
	}

	[Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f), LabelText("Удалить все сохранения")]
	private void DeleteAllSaveData()
	{
		if (File.Exists(savePath))
		{
			File.Delete(savePath);
			Debug.Log("[SaveDataEditor] Сейв файл удалён.");
		}

		currentSaveData = new SaveData();

		playerLevel = 1;
		playerExp = 0f;
		playerGold = 0;
		inventoryLevel = 1;

		var json = JsonUtility.ToJson(currentSaveData, true);
		File.WriteAllText(savePath, json);

		if (Application.isPlaying && SaveManager.Instance != null)
		{
			SaveManager.Instance.LoadGame();

			var chestController = GameObject.FindObjectOfType<СhestController>();
			if (chestController != null)
			{
				foreach (var chest in chestController.chests)
				{
					chest.ClearSlots();
				}
			}

			Debug.Log("[SaveDataEditor] Всё удалено и runtime очищен.");
		}
		else
		{
			Debug.Log("[SaveDataEditor] Всё удалено. Новый пустой файл сохранён.");
		}
	}

	[Button(ButtonSizes.Medium), GUIColor(1f, 0.5f, 0.5f), LabelText("Удалить PlayerPrefs НАСТРОЕК")]
	private void DeleteSettingsPlayerPrefs()
	{
		PlayerPrefs.DeleteKey("MouseSensitivity");
		PlayerPrefs.DeleteKey("FpsLimit");
		PlayerPrefs.DeleteKey("ScreenResolution");
		PlayerPrefs.DeleteKey("QualityLevel");
		PlayerPrefs.DeleteKey("BrightnessLevel");
		PlayerPrefs.DeleteKey("MasterVolume");
		PlayerPrefs.DeleteKey("MusicVolume");
		PlayerPrefs.DeleteKey("SfxVolume");
		PlayerPrefs.Save();

		Debug.Log("[SaveDataEditor] PlayerPrefs, связанные с настройками, удалены.");
	}

	[Button(ButtonSizes.Medium), GUIColor(1f, 0.4f, 0.4f), LabelText("Удалить ВСЕ игровые PlayerPrefs (кроме настроек)")]
	private void DeleteOnlyGamePlayerPrefs()
	{
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

		PlayerPrefs.DeleteAll();
		
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
		Debug.Log("[SaveDataEditor] Все игровые PlayerPrefs удалены, настройки сохранены.");
	}

	
}
#endif
