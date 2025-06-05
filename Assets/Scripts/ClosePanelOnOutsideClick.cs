using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class ClosePanelOnOutsideClick : MonoBehaviour
{
    [Header("UI References")]
    public HamburgerMenu menuPanel;

    [Header("Settings")]
    public float cooldownDuration = 0.5f;

    private RectTransform panelRect;
    private InputAction clickAction;
    private Canvas canvas;
    private GraphicRaycaster raycaster;
    private float lastCloseTime = -1f;
    private bool isPanelClosing; // Tracks if panel is in closing animation

    void Awake()
    {
        panelRect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        if (canvas != null)
            raycaster = canvas.GetComponent<GraphicRaycaster>();
    }

    void OnEnable()
    {
        // Create and enable input action
        clickAction = new InputAction(
            binding: "<Mouse>/leftButton",
            interactions: "press(behavior=1)");
        clickAction.AddBinding("<Touchscreen>/touch*/press");
        clickAction.performed += HandleClick;
        clickAction.Enable();
    }

    void OnDisable()
    {
        // Clean up input action
        clickAction.Disable();
        clickAction.performed -= HandleClick;
        clickAction.Dispose();
    }

    void Update()
    {
        // Update panel closing status
        if (isPanelClosing &&
            menuPanel != null &&
            menuPanel.dropdownPanel != null &&
            !menuPanel.dropdownPanel.activeSelf)
        {
            isPanelClosing = false;
        }
    }

    private void HandleClick(InputAction.CallbackContext context)
    {
        // Skip if panel is in closing animation
        if (isPanelClosing) return;

        // Skip if within cooldown period
        if (Time.time - lastCloseTime < cooldownDuration) return;

        // Skip if menu not open
        if (menuPanel == null ||
            menuPanel.dropdownPanel == null ||
            !menuPanel.dropdownPanel.activeSelf)
            return;

        Vector2 clickPosition = GetClickPosition(context);

        if (!IsPointerOverPanel(clickPosition))
        {
            // Mark panel as closing
            isPanelClosing = true;
            lastCloseTime = Time.time;
            menuPanel.ToggleMenu();
        }
    }

    private Vector2 GetClickPosition(InputAction.CallbackContext context)
    {
        return context.control.device switch
        {
            Touchscreen touchscreen => touchscreen.primaryTouch.position.ReadValue(),
            _ => Mouse.current.position.ReadValue()
        };
    }

    private bool IsPointerOverPanel(Vector2 screenPosition)
    {
        // Check direct rectangle hit
        if (RectTransformUtility.RectangleContainsScreenPoint(panelRect, screenPosition))
            return true;

        // Check child UI elements
        if (raycaster != null)
        {
            var eventData = new PointerEventData(EventSystem.current)
            {
                position = screenPosition
            };

            var results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject.transform.IsChildOf(panelRect.transform))
                {
                    return true;
                }
            }
        }
        return false;
    }
}