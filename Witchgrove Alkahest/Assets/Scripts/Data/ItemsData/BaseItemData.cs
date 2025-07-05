using UnityEngine;
using UnityEngine.Serialization;

// Base item class
public abstract class BaseItemData : ScriptableObject
{
	public string id;
	public string displayName;
	public Sprite icon;
	public int maxStack = 5;
	public int price = 0;
	
	public override string ToString()
	{
		return displayName;
	}
}