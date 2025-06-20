// CauldronUI.cs
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// UI for the cauldron: craft slots and water level only. Results go directly to inventory.
/// </summary>
public class CauldronUI : MonoBehaviour
{
	[Tooltip("Assign craft CellUI components ")]
	[SerializeField] private CellUI[] craftCells;
	
	[Space(15f)]
	[SerializeField] private Button craftButton;
	[SerializeField] private Cauldron cauldronController;

	private void OnEnable()
	{
		craftButton.onClick.AddListener(Craft);
	}

	private void OnDisable()
	{
		craftButton.onClick.RemoveListener(Craft);
	}

	private void Start()
	{
		var craftSlots = cauldronController.craftCellSlots;
		for (int i = 0; i < craftCells.Length && i < craftSlots.Count; i++)
		{
			craftCells[i].Setup(craftSlots[i], craftSlots, i);
		}

		RefreshCellsUI(); 
	}

	private void Craft()
	{
		cauldronController.TryCraft();
	}
	
	public void RefreshCellsUI()
	{
		foreach (var cell in craftCells)
			cell.UpdateCellUI(); 
	}
}