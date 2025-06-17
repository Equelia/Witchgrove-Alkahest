using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Requires the object to have a trigger collider.
/// Base class of interactable items
/// </summary>

[RequireComponent(typeof(Collider))]
public abstract class InteractableItem : MonoBehaviour
{
	public virtual void Interact()
	{
		InventorySystem.Instance.inventoryUI.OpenInventory();
	}
}