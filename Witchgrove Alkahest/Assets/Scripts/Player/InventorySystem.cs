using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Types of collectible ingredients.
/// </summary>
public enum ItemType
{
    Herb,
    Mushroom,
    Crystal,
    Вода,
    СмущенноеЗелье,
    ЗельеЗдоровья
}

/// <summary>
/// Represents a single slot in the inventory.
/// </summary>
[Serializable]
public class CellSlot
{
    public ItemType Type;
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
            inventorySlots.Add(new CellSlot { Type = default, Count = 0 });
        }
    }

    /// <summary>
    /// Try to add an item of the given type.
    /// Returns true if added successfully; false if inventory is full.
    /// </summary>
    public bool AddItem(ItemType type)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            
            // 1) Try stacking onto existing slot
            if (slot.Count > 0 && slot.Type == type && slot.Count < maxStack)
            {
                slot.Count++;
                inventoryUI.UpdateSlotUI(i);
                Debug.Log($"[Inventory] Stacked one more {type}. Now: {slot.Count}");
                return true;
            }
            // 2) Get in the empty slot
            if (slot.Count == 0)
            {
                slot.Type = type;
                slot.Count = 1;
                inventoryUI.UpdateSlotUI(i);
                Debug.Log($"[Inventory] Added new slot for {type}.");
                return true;
            }
        }
        
        // 3) Inventory full
        Debug.LogWarning($"[Inventory] Cannot add {type}: inventory full.");
        return false;
    }
    
    public bool TryConsumeItem(ItemType type, int amount)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            var slot = inventorySlots[i];
            
            if (slot.Type == type && slot.Count >= amount)
            {
                slot.Count -= amount;
                if (slot.Count == 0)
                    slot.Type = default;

                inventoryUI.UpdateSlotUI(i);
                return true;
            }
        }
        return false;
    }
}
