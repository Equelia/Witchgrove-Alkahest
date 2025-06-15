using System;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// Interface for every Inventory
/// </summary>
public interface IExternalInventoryReceiver
{
    bool CanReceiveItem(BaseItemData item);
    bool ReceiveItem(BaseItemData item, int amount);
}


/// <summary>
/// Represents a single slot in the inventory.
/// </summary>
[Serializable]
public class CellSlot
{
    public BaseItemData ItemData;
    public int Count;
}

/// <summary>
/// Singleton manager that handles inventory logic: adding, using items, and notifying UI.
/// </summary>
public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; private set; }

    [Header("Inventory Settings")]
    [Tooltip("Maximum number of slots allowed")]
    [SerializeField] private int maxSlots = 4;
    [Tooltip("Maximum stack size per slot")]
    public int maxStack = 5;
    
    [HideInInspector] public List<CellSlot> inventorySlots;
    
    public InventoryUI inventoryUI;
    public IExternalInventoryReceiver CurrentExternalReceiver;

    
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
        inventorySlots = new List<CellSlot>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
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
            if (slot.Count > 0 && slot.ItemData == item && slot.Count < maxStack)
            {
                slot.Count++;
                inventoryUI.UpdateSlotUI(i);
                Debug.Log($"[Inventory] Stacked one more {item}. Now: {slot.Count}");
                return true;
            }
            // 2) Get in the empty slot
            if (slot.Count == 0)
            {
                slot.ItemData = item;
                slot.Count = 1;
                inventoryUI.UpdateSlotUI(i);
                Debug.Log($"[Inventory] Added new slot for {item}.");
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

                inventoryUI.UpdateSlotUI(i);
                return true;
            }
        }
        return false;
    }
}
