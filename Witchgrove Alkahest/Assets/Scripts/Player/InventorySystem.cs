using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for every Inventory
/// </summary>
public interface IExternalInventoryReceiver
{
    List<CellSlot> GetAllSlots();
    bool TryAddOneItem(BaseItemData item);
}


/// <summary>
/// Represents a single slot in the inventory.
/// </summary>
[Serializable]
public class CellSlot
{
    private BaseItemData _itemData;
    private int _count;

    //Fire when something changes in a slot
    public event Action<CellSlot> OnSlotChanged;

    public BaseItemData ItemData
    {
        get => _itemData;
        set
        {
            if (_itemData == value) return;
            _itemData = value;
            OnSlotChanged?.Invoke(this);
        }
    }

    public int Count
    {
        get => _count;
        set
        {
            if (_count == value) return;
            _count = value;
            OnSlotChanged?.Invoke(this);
        }
    }
}

/// <summary>
/// Singleton manager that handles inventory logic: adding, using items, and notifying UI.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }
    
    public InventoryUI inventoryUI;
    public IExternalInventoryReceiver CurrentExternalReceiver;
    [HideInInspector] public List<CellSlot> inventorySlots;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize empty slots
        inventorySlots = new List<CellSlot>(4);
        for (int i = 0; i < 4; i++)
        {
            inventorySlots.Add(new CellSlot { ItemData = default, Count = 0 });
        }
    }

    /// <summary>
    /// Try to add an item of the given type.
    /// Returns true if added successfully; false if inventory is full.
    /// </summary>
    public bool AddItem(BaseItemData item)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            
            // 1) Try stacking onto existing slot
            if (slot.Count > 0 && slot.ItemData == item && slot.Count < slot.ItemData.maxStack)
            {
                slot.Count++;
                return true;
            }
            // 2) Get in the empty slot
            if (slot.Count == 0)
            {
                slot.ItemData = item;
                slot.Count = 1;
                return true;
            }
        }
        
        // 3) Inventory full
        Debug.LogWarning($"[Inventory] Cannot add {item}: inventory full.");
        return false;
    }
    
    public bool TryConsumeItem(BaseItemData item, int amount)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            
            if (slot.ItemData == item && slot.Count >= amount)
            {
                slot.Count -= amount;
                if (slot.Count == 0)
                    slot.ItemData = default;
                return true;
            }
        }
        return false;
    }
}
