using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
	// 1) Player
	public int playerLevel;
	public float playerExp;
	public int playerGold;
	public int playerInventoryLevel;

	// 2) Player inventory
	[System.Serializable]
	public struct SlotData
	{
		public string itemId;
		public int count;
	}
	public List<SlotData> playerInventory = new List<SlotData>();

	// 3) Сhests data
	public List<ChestSave> chests = new List<ChestSave>();

	// 4) Task board comepeleted quests
	public List<string> completedQuestIds = new List<string>();

	// 5) Cauldron slots
	public List<SlotData> cauldronCraftSlots = new List<SlotData>();
	
	public List<SlotData> basketSlots = new List<SlotData>();
}