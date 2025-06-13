using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Types of collectible ingredients.
/// </summary>
public enum IngredientType
{
    Herb,
    Mushroom,
    Crystal,
}

/// <summary>
/// Represents a single slot in the inventory.
/// </summary>
[Serializable]
public class InventorySlot
{
    public IngredientType Type;
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
    [SerializeField] private int maxStack = 5;

    private List<InventorySlot> slots;
    
    public event Action OnInventoryChanged;

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
        slots = new List<InventorySlot>(maxSlots);
        for (int i = 0; i < maxSlots; i++)
        {
            slots.Add(new InventorySlot { Type = default, Count = 0 });
        }
    }

    /// <summary>
    /// Try to add an item of the given type.
    /// Returns true if added successfully; false if inventory is full.
    /// </summary>
    public bool AddItem(IngredientType type)
    {
        // 1) Try stacking onto existing slot
        foreach (var slot in slots)
        {
            if (slot.Count > 0 && slot.Type == type && slot.Count < maxStack)
            {
                slot.Count++;
                OnInventoryChanged?.Invoke();
                Debug.Log($"[Inventory] Stacked one more {type}. Now: {slot.Count}");
                return true;
            }
        }

        // 2) Try placing into empty slot
        foreach (var slot in slots)
        {
            if (slot.Count == 0)
            {
                slot.Type = type;
                slot.Count = 1;
                OnInventoryChanged?.Invoke();
                Debug.Log($"[Inventory] Added new slot for {type}.");
                return true;
            }
        }

        // 3) Inventory full
        Debug.LogWarning($"[Inventory] Cannot add {type}: inventory full.");
        return false;
    }

    /// <summary>
    /// Use (or remove) one item of the given type from inventory.
    /// </summary>
    public void UseItem(IngredientType type)
    {
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (slot.Count > 0 && slot.Type == type)
            {
                slot.Count--;
                Debug.Log($"[Inventory] Used one {type}. Remaining: {slot.Count}");
                // If slot is now empty, clear its type
                if (slot.Count == 0)
                    slots[i].Type = default;

                OnInventoryChanged?.Invoke();
                return;
            }
        }

        Debug.LogWarning($"[Inventory] No item of type {type} to use.");
    }

    public IReadOnlyList<InventorySlot> GetSlots() => slots;
}
