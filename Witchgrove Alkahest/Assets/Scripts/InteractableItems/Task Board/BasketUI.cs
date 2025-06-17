using UnityEngine;

public class BasketUI : MonoBehaviour
{
    [SerializeField] private Basket basketController;
    
    [Tooltip("Assign Basket CellUI components ")] 
    [SerializeField] private CellUI[] basketCells;
	

    private void Start()
    {
        for (int i = 0; i < basketCells.Length; i++)
        {
            basketCells[i].Setup(basketCells[i].SlotData, basketController.basketCells, i);
        }
    }

    public void RefreshCellsUI()
    {
        foreach (var cell in basketCells) cell.UpdateCellUI();
    }
}
