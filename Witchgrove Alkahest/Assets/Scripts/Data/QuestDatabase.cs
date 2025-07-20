using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class QuestData 
{
	public string questId;
	[TextArea] public string description;
	public BaseItemData  requiredItem;
	public int requiredCount = 1;
	public float expAmount;
	public int goldAmount;
}

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
	public int requiredLevel = 1;
	[Space (10f)]
	public List<QuestData> quests;
}