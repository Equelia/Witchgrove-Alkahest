using System;
using UnityEngine;

public class BasketUI : MonoBehaviour
{
    [Tooltip("Assign Basket CellUI components ")] 
    [SerializeField] private CellUI[] basketCells;
    
    [SerializeField] private Basket basketController;
    [SerializeField] private TaskBoardUI taskBoardUI;
	
    private void OnEnable()
    {
        foreach (var slot in basketController.GetAllSlots())
            slot.OnSlotChanged += HandleSlotChanged;
    }

    private void OnDisable()
    {
        foreach (var slot in basketController.GetAllSlots())
            slot.OnSlotChanged -= HandleSlotChanged;
    }

    private void Start()
    {
        var slots = basketController.GetAllSlots();  
        for (int i = 0; i < basketCells.Length && i < slots.Count; i++) 
            basketCells[i].Setup(slots[i], slots, i);
    }
    
    private void HandleSlotChanged(CellSlot changedSlot)
    {
        RefreshCellsUI();
    }

    public void RefreshCellsUI()
    {
        for (int i = 0; i < basketCells.Length; i++)
        {
            basketCells[i].UpdateCellUI();
            taskBoardUI.UpdateAvailableItemsCount();
        }
    }
}
