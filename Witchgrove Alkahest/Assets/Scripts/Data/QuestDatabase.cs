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
}

[CreateAssetMenu(menuName = "Quest/QuestDatabase")]
public class QuestDatabase : ScriptableObject
{
	public List<QuestData> quests;
}