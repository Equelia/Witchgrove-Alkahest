using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TraderUI : MonoBehaviour
{
	[Header("Trader UI")]
	[SerializeField] private GameObject tradeItemPrefab;
	[SerializeField] private Transform contentRoot;
	[SerializeField] private Button sellButton;

	[Tooltip("Assign Trader CellUI components "), Space(15F)] 
	[SerializeField] private CellController[] soldCells;
	[SerializeField] private Trader trader;

	private void Start()
	{
		var slots = trader.GetAllSlots();  

		for (int i = 0; i < soldCells.Length && i < slots.Count; i++)
		{
			soldCells[i].Setup(slots[i], slots, i);
		}

		InstantiateTraderItems();
		sellButton.onClick.AddListener(SellAll);
	} 

	public void InstantiateTraderItems()
	{
		Clear();

		foreach (var item in trader.GetItemsForSale())
		{
			var go = Instantiate(tradeItemPrefab, contentRoot);
			go.GetComponent<TraderItemUI>().Setup(item, trader);
		}

		gameObject.SetActive(true);
	}

	private void Clear()
	{
		foreach (Transform child in contentRoot)
			Destroy(child.gameObject);
	}
	
	public void SellAll()
	{
		trader.SellAllPotionsInSlots();
	}
}