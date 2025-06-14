using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Raycasts from screen center to detect pickupable items,
/// shows their name in the UI, and on “E” adds them to inventory.
/// </summary>
public class ObjectInteractor : MonoBehaviour
{
    [Header("Interaction Settings")]
    [Tooltip("Max distance at which we can interact")]
    [SerializeField] private float interactDistance = 3f;
    [Tooltip("Layer mask for pickupable objects")]
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

        if (pickupableItem != null && Input.GetKeyDown(KeyCode.E))
            PickUpHoveredItem();

        if (interactableItem != null && Input.GetKeyDown(KeyCode.E))
        {
            interactableItem.Interact();            
            objectNameTextHolder.SetActive(false);
        }
    }
    
    private void HandleHover()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            // Try to get a PickUpItem component
            if (hit.collider.TryGetComponent<PickupableItem>(out var pickupable_item))
            {
                pickupableItem = pickupable_item;
                objectNameText.text = pickupable_item.type.ToString();
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);
                return;
            }
            if (hit.collider.TryGetComponent<InteractableItem>(out var interactable_item))
            {
                interactableItem = interactable_item;
                objectNameText.text = interactable_item.gameObject.name;
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);
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
        objectNameTextHolder.SetActive(false);
    }


    /// <summary>
    /// Adds the hovered item to inventory, deactivates it, and hides UI.
    /// </summary>
    private void PickUpHoveredItem()
    {
        bool added = InventorySystem.Instance.AddItem(pickupableItem.type);

        if (!added)
        {
            Debug.Log("[ObjectInteractor] Inventory full, cannot pick up item.");
            return;
        }

        pickupableItem.gameObject.SetActive(false);
        pickupableItem = null;
        objectNameTextHolder.SetActive(false);
    }
}
