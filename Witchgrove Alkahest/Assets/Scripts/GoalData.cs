using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Goal Data")]
public class GoalData : ScriptableObject
{
	public string description;
	public string[] dialogStrings;
}