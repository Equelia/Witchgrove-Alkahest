using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CauldronUI : MonoBehaviour
{
	[Tooltip("Assign craft CellUI components ")]
	[SerializeField] private CellUI[] craftCells;

	[Tooltip("Assign craft CellUI components ")]
	[SerializeField] private CellUI[] resultCells;
	
	[Header("Water Level")]
	[SerializeField] private Image waterLevelImage;
	[SerializeField] private Button addWaterButton;
	
	[Space(15f)]
	[SerializeField] private Button craftButton;
	[SerializeField] private Cauldron cauldronController;

	private CellUI[] allCells;
	
	private void OnEnable()
	{
		craftButton.onClick.AddListener(Craft);
		addWaterButton.onClick.AddListener(AddWater);
	}

	private void OnDisable()
	{
		craftButton.onClick.RemoveListener(Craft);
		addWaterButton.onClick.RemoveListener(AddWater);
	}
	
	private void Awake()
	{
		allCells = new CellUI[craftCells.Length + resultCells.Length];
		
		for (int i = 0; i < craftCells.Length; i++)
			allCells[i] = craftCells[i];

		for (int i = 0; i < resultCells.Length; i++)
			allCells[craftCells.Length + i] = resultCells[i];
	}


	private void Start()
	{
		var craftSlots = cauldronController.craftCellSlots;
		var resultSlots = cauldronController.resultCellSlots;
		var allSlots = cauldronController.GetAllSlots();
		
		for (int i = 0; i < craftCells.Length && i < craftSlots.Count; i++)
			craftCells[i].Setup(craftSlots[i], craftSlots, i);

		for (int i = 0; i < resultCells.Length && i < resultSlots.Count; i++)
			resultCells[i].Setup(resultSlots[i], resultSlots, i);
		
		for (int i = 0; i < allCells.Length && i < allSlots.Count; i++)
			allCells[i].Setup(allSlots[i], allSlots, i);
		
		RefreshCellsUI();
	}

	private void Craft()
	{
		cauldronController.TryCraft();
		RefreshCellsUI();
	}

	private void AddWater()
	{
		if (cauldronController.TryAddWater())
			UpdateWaterUI();
	}
	
	public void RefreshCellsUI()
	{
		foreach (var cell in craftCells) cell.UpdateCellUI();
		foreach (var cell in resultCells) cell.UpdateCellUI();
		UpdateWaterUI();
	}

	private void UpdateWaterUI()
	{
		waterLevelImage.fillAmount = (float)cauldronController.currentWaterAmount / cauldronController.maxWaterAmount;
	}
}
