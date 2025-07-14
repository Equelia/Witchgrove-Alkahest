using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Requires the object to have a trigger collider.
/// Base class of interactable items
/// </summary>

[RequireComponent(typeof(Collider))]
public abstract class InteractableItem : MonoBehaviour
{
	[SerializeField] private ObjectInteractor objectInteractor;
	
	public virtual void Interact()
	{
		PlayerInventorySystem.Instance.playerInventoryUI.inventoryWindowManager.OpenInventory();
		
		if (objectInteractor != null)
			objectInteractor.BlockInteractionThisFrame = true;
	}
}