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
    private PickUpItem hoveredItem;

    void Awake()
    {
        mainCamera = Camera.main;
        objectNameTextHolder.gameObject.SetActive(false);
    }

    void Update()
    {
        HandleHover();

        if (hoveredItem != null && Input.GetKeyDown(KeyCode.E))
            PickUpHoveredItem();
    }

    /// <summary>
    /// Raycasts from the center of the screen. 
    /// If we hit a PickUpItem, cache it and show its name.
    /// Otherwise clear cache and hide the UI.
    /// </summary>
    private void HandleHover()
    {
        // Ray from viewport center (0.5,0.5)
        Ray ray = mainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer))
        {
            // Try to get a PickUpItem component
            if (hit.collider.TryGetComponent<PickUpItem>(out var item))
            {
                hoveredItem = item;
                objectNameText.text = item.type.ToString();
                if (!objectNameTextHolder.gameObject.activeSelf)
                    objectNameTextHolder.gameObject.SetActive(true);
                return;
            }
        }

        // Nothing valid hit → clear
        hoveredItem = null;
        objectNameTextHolder.gameObject.SetActive(false);
    }

    /// <summary>
    /// Adds the hovered item to inventory, deactivates it, and hides UI.
    /// </summary>
    private void PickUpHoveredItem()
    {
        bool added = InventorySystem.Instance.AddItem(hoveredItem.type);

        if (!added)
        {
            Debug.Log("[ObjectInteractor] Inventory full, cannot pick up item.");
            return;
        }

        hoveredItem.gameObject.SetActive(false);
        hoveredItem = null;
        objectNameTextHolder.gameObject.SetActive(false);
    }
}
