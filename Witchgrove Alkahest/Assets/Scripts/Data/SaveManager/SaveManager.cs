using UnityEngine;
using System.IO;
using System.Collections.Generic;

[DefaultExecutionOrder(100)]
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }
    private string savePath;

    [Header("Systems")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private СhestController chestSystem; 
    [SerializeField] private Basket basket;
    [SerializeField] private TaskBoard taskBoard;
    [SerializeField] private Cauldron cauldron;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        savePath = Path.Combine(Application.persistentDataPath, "savegame.json");
        LoadGame();  // load everything on start
    }

    private void OnApplicationQuit()
    {
        SaveGame(); // Save everything when leaving
    }

    // Save all modules
    public void SaveGame()
    {
        var data = LoadOrCreateSaveData();
        SavePlayerModule(data);
        SaveInventoryModule(data);
        SaveChestsModule(data);
        SaveQuestsModule(data);
        SaveCauldronModule(data);
        SaveBasketModule(data);
        WriteToFile(data);
        Debug.Log($"[SaveManager] Game saved to {savePath}");
    }

    // Load all modules
    public void LoadGame()
    {
        var data = LoadOrCreateSaveData();
        LoadPlayerModule(data);
        LoadInventoryModule(data);
        LoadChestsModule(data);
        LoadQuestsModule(data);
        LoadCauldronModule(data);
        LoadBasketModule(data);
        Debug.Log($"[SaveManager] Game loaded from {savePath}");
    }
    
    // PlayerData
    public void SavePlayerModule(SaveData data)
    {
        data.playerLevel = playerData.Level;
        data.playerExp   = playerData.TotalExp;
    }

    public void LoadPlayerModule(SaveData data)
    {
        playerData.Level = Mathf.Max(1, data.playerLevel); 
        playerData.TotalExp = data.playerExp;
    }

    
    // Inventory
    public void SaveInventoryModule(SaveData data)
    {
        data.playerInventory.Clear();
        foreach (var slot in PlayerInventorySystem.Instance.GetAllSlots())
        {
            if (slot.Count == 0) continue;
            data.playerInventory.Add(new SaveData.SlotData {
                itemId = slot.ItemData.id,
                count  = slot.Count
            });
        }
    }

    public void LoadInventoryModule(SaveData data)
    {
        PlayerInventorySystem.Instance.ClearAllSlots();
        foreach (var sd in data.playerInventory)
        {
            var item = ItemDatabase.Instance.GetItemById(sd.itemId);
            PlayerInventorySystem.Instance.AddToFirstEmpty(item, sd.count);
        }
    }


    // Chests
    public void SaveChestsModule(SaveData data)
    {
        data.chests.Clear();
        foreach (var chest in chestSystem.chests)
        {
            var save = new ChestSave {
                chestId = chest.ChestId,
                slots   = new List<SaveData.SlotData>()
            };
            foreach (var slot in chest.GetAllSlots())
            {
                if (slot.Count == 0) continue;
                save.slots.Add(new SaveData.SlotData {
                    itemId = slot.ItemData.id,
                    count  = slot.Count
                });
            }
            data.chests.Add(save);
        }
    }
    
    public void LoadChestsModule(SaveData data)
    {
        foreach (var save in data.chests)
        {
            var chest = chestSystem.GetChestById(save.chestId);
            if (chest == null) continue;

            chest.ClearAllSlots();
            foreach (var sd in save.slots)
            {
                var item = ItemDatabase.Instance.GetItemById(sd.itemId);
                chest.AddToFirstEmpty(item, sd.count);
            }
        }
    }
    
    // Basket

    public void SaveBasketModule(SaveData data)
    {
        data.basketSlots.Clear();
        foreach (var slot in basket.GetAllSlots())
        {
            if (slot.Count == 0) continue;
            data.basketSlots.Add(new SaveData.SlotData {
                itemId = slot.ItemData.id,
                count  = slot.Count
            });
        }
    }

    public void LoadBasketModule(SaveData data)
    {
        basket.ClearAllSlots();
        foreach (var sd in data.basketSlots)
        {
            var item = ItemDatabase.Instance.GetItemById(sd.itemId);
            basket.AddToFirstEmpty(item, sd.count);
        }
    }

    // TaskBoard / Quests
    public void SaveQuestsModule(SaveData data)
    {
        data.completedQuestIds = taskBoard
            .GetCompletedQuests()
            .ConvertAll(q => q.questId);
    }
    
    public void LoadQuestsModule(SaveData data)
    {
        taskBoard.SetCompletedQuestsByIds(data.completedQuestIds);
    }
    
    // Cauldron
    public void SaveCauldronModule(SaveData data)
    {
        data.cauldronCraftSlots.Clear();
        foreach (var slot in cauldron.GetAllSlots())
        {
            if (slot.Count == 0) continue;
            data.cauldronCraftSlots.Add(new SaveData.SlotData {
                itemId = slot.ItemData.id,
                count  = slot.Count
            });
        }
    }

    public void LoadCauldronModule(SaveData data)
    {
        cauldron.ClearAllSlots();
        foreach (var sd in data.cauldronCraftSlots)
        {
            var item = ItemDatabase.Instance.GetItemById(sd.itemId);
            cauldron.AddToFirstEmpty(item, sd.count);
        }
    }
    
        
    // Overall utilities
    private SaveData LoadOrCreateSaveData()
    {
        if (File.Exists(savePath))
        {
            var json = File.ReadAllText(savePath);
            return JsonUtility.FromJson<SaveData>(json);
        }
        else
        {
            return new SaveData();
        }
    }

    private void WriteToFile(SaveData data)
    {
        var json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(savePath, json);
    }
}
