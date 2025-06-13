using UnityEngine;

/// <summary>
/// Attach to pickupable objects.
/// Requires the object to have a trigger collider.
/// </summary>

[RequireComponent(typeof(Collider))]
public class PickUpItem : MonoBehaviour
{
	[Tooltip("Type of ingredient this object yields")]
	public IngredientType type;
}