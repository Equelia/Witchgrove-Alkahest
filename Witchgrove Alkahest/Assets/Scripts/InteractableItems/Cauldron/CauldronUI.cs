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

	private void Start()
	{
		UpdateWaterUI();
		
		for (int i = 0; i < craftCells.Length; i++)
		{
			craftCells[i].Setup(craftCells[i].SlotData, cauldronController.craftCellSlots, i);
		}
		for (int i = 0; i < resultCells.Length; i++)
		{
			resultCells[i].Setup(resultCells[i].SlotData, cauldronController.resultCellSlots, i);
		}
		
	}

	private void Craft()
	{
		cauldronController.TryCraft();
		RefreshCraftUI();
	}

	private void AddWater()
	{
		if (cauldronController.TryAddWater())
			UpdateWaterUI();
	}
	
	private void RefreshCraftUI()
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
