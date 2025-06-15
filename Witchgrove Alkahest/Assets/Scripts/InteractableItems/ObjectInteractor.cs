using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Raycasts from screen center to detect interactable items,
/// shows their name in the UI, and on “E” interacts with them.
/// </summary>
public class ObjectInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Max distance at which we can interact")]
    [SerializeField] private float interactDistance = 3f;
    [Tooltip("Layer mask for interactable objects")]
    [SerializeField] private LayerMask interactLayer;

    [Header("UI")]
    [Tooltip("UI Text to display hovered object name")]
    [SerializeField] private GameObject objectNameTextHolder;
    [SerializeField] private TMP_Text objectNameText;

    private Camera mainCamera;
    private PickupableItem pickupableItem;
    private InteractableItem interactableItem;

    void Awake()
    {
        mainCamera = Camera.main;
        objectNameTextHolder.SetActive(false);
    }

    void Update()
    {
        if (InventorySystem.Instance.inventoryUI.IsOpen)
        {
            ClearHover();
            return;
        }
        
        HandleHover();

        //Interact with pickupable item
        if (pickupableItem != null && Input.GetKeyDown(KeyCode.E))
            PickUpHoveredItem();
        else if (interactableItem != null && Input.GetKeyDown(KeyCode.E))         //Interact with interactable item
        {
            interactableItem.Interact();        
            objectNameTextHolder.SetActive(false);
            
            if (interactableItem is IExternalInventoryReceiver receiver)
                InventorySystem.Instance.CurrentExternalReceiver = receiver;
            else
                InventorySystem.Instance.CurrentExternalReceiver = null;
        }
    }
    
    
    /// <summary>
    /// Check what's player hovering over to execute possible interactions 
    /// </summary>
    private void HandleHover()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            // Try to get a PickupableItem component
            if (hit.collider.TryGetComponent<PickupableItem>(out var pickupable_item))
            {
                pickupableItem = pickupable_item;
                objectNameText.text = pickupable_item.ingredientData.ToString();
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);

                interactableItem = null;
                return;
            }
            // Then try to get a InteractableItem component
            if (hit.collider.TryGetComponent<InteractableItem>(out var interactable_item))
            {
                interactableItem = interactable_item;
                objectNameText.text = interactable_item.gameObject.name;
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);
                
                pickupableItem = null;
                return;
            }
        }

        pickupableItem = null;
        interactableItem = null;
        objectNameTextHolder.SetActive(false);
    }

    private void ClearHover()
    {
        pickupableItem = null;
        interactableItem = null;
        objectNameTextHolder.SetActive(false);
    }


    /// <summary>
    /// Adds the hovered item to inventory, deactivates it, and hides UI.
    /// </summary>
    private void PickUpHoveredItem()
    {
        bool added = InventorySystem.Instance.AddItem(pickupableItem.ingredientData);

        if (!added)
        {
            Debug.Log("[ObjectInteractor] Inventory full, cannot pick up item.");
            return;
        }

        if (pickupableItem.consumable)
        {
            pickupableItem.gameObject.SetActive(false);
            pickupableItem = null;
            objectNameTextHolder.SetActive(false);
        }
    }
}
