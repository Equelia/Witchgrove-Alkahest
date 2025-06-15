using UnityEngine;

/// <summary>
/// Attach to pickupable objects.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PickupableItem : InteractableItem
{
	[Tooltip("Type of ingredient this object yields")]
	public ItemType type;
	public bool consumable = true;
}