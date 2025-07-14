using System;
using System.Linq;
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
    [Space, SerializeField] private InventoryWindowManager windowManager;
    
    [Header("Tutorial")]
    [SerializeField] private UIWindowGroup uiWindowGroup;
    
    [NonSerialized] public bool BlockInteractionThisFrame = false;
    
    private Camera mainCamera;
    private InteractableItem selectedItem;

    void Awake()
    {
        mainCamera = Camera.main;
        objectNameTextHolder.SetActive(false);
    }

    private void Start()
    {
        uiWindowGroup?.Show();
    }

    void Update()
    {
        bool isUIOpen = windowManager.panels.Any(entry => entry.panel.activeSelf) ||  windowManager.IsInventoryOpen;

        if (isUIOpen)
        {
            if (!BlockInteractionThisFrame)
                ClearHover();

            return;
        }
        
        HandleHover();
        
        if (BlockInteractionThisFrame)
            return;

        //Interact with pickupable item
        if (selectedItem != null && Input.GetKeyDown(KeyCode.E))
        {
            if (selectedItem.TryGetComponent<SpecificIngredientTutorial>(out var specific_ingredient_tutorial))
                specific_ingredient_tutorial.Interact();
            if (selectedItem.TryGetComponent<PickupableItem>(out var pickupable_item))
                PickUpHoveredItem(pickupable_item);
            if (selectedItem.TryGetComponent<InteractableItem>(out var interactable_item))
            {
                interactable_item.Interact();
                objectNameTextHolder.SetActive(false);

                var receiver = interactable_item.GetComponent<IExternalInventoryReceiver>();
                if (receiver == null)
                    receiver = interactable_item.GetComponentInChildren<IExternalInventoryReceiver>();

                PlayerInventorySystem.Instance.CurrentExternalReceiver = receiver;
            }
        }
    }
    
    private void LateUpdate()
    {
        BlockInteractionThisFrame = false;
    }
    
    
    /// <summary>
    /// Check what's player hovering over to execute possible interactions 
    /// </summary>
    private void HandleHover()
    {
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            Vector3 origin = mainCamera.transform.position;
            Vector3 dirToHit = hit.point - origin;

            if (Physics.Raycast(origin, dirToHit.normalized, out RaycastHit blockCheck, dirToHit.magnitude))
            {
                if (!blockCheck.collider.gameObject.Equals(hit.collider.gameObject))
                {
                    selectedItem = null;
                    objectNameTextHolder.SetActive(false);
                    return;
                }
            }
            
            // Try to get a PickupableItem component
            if (hit.collider.TryGetComponent<PickupableItem>(out var pickupable_item))
            {
                selectedItem = pickupable_item;
                objectNameText.text = pickupable_item.ingredientData.ToString();
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);
                return;
            }
            // Then try to get a InteractableItem component
            if (hit.collider.TryGetComponent<InteractableItem>(out var interactable_item))
            {
                selectedItem = interactable_item;
                objectNameText.text = interactable_item.gameObject.name;
                if (!objectNameTextHolder.activeSelf)
                    objectNameTextHolder.SetActive(true);
                return;
            }
        }

        selectedItem = null;
        objectNameTextHolder.SetActive(false);
    }

    private void ClearHover()
    {
        selectedItem = null;
        objectNameTextHolder.SetActive(false);
    }


    /// <summary>
    /// Adds the hovered item to inventory, deactivates it, and hides UI.
    /// </summary>
    private void PickUpHoveredItem(PickupableItem pickupableItem)
    {
        pickupableItem.Interact();
        
        bool added = PlayerInventorySystem.Instance.TryAddOneItem(pickupableItem.ingredientData);

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
        
        SoundManager.Instance.PlaySound("CellPop");
    }
}
