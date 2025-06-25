using UnityEngine;
using UnityEngine.Serialization;

/// <summary>
/// Attach to pickupable objects.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PickupableItem : InteractableItem
{ 
	[Tooltip("Type of ingredient this object yields")]
	public IngredientData ingredientData;
	public bool consumable = true;
}