using UnityEngine;
using UnityEngine.UI;


public class HamburgerMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dropdownPanel;

    [Header("Animation Settings")]
    [Tooltip("Cooldown duration in seconds")]
    public float cooldown = 0.5f;

    private Animator menuAnimator;
    private Button menuButton;
    private bool isOpen = false;
    private bool isOnCooldown;

    void Start()
    {
        menuAnimator = dropdownPanel.GetComponent<Animator>();
        menuButton = GetComponent<Button>(); // Reference to attached button

        menuAnimator.enabled = false;
        dropdownPanel.SetActive(false);

        isOnCooldown = false;
    }

    public void ToggleMenu()
    {
        if (isOnCooldown) return;

        StartCooldown();

        isOpen = !isOpen;

        // Always enable animator before playing animations
        menuAnimator.enabled = true;

        if (isOpen)
        {
            // Reset state before activation
            menuAnimator.ResetTrigger("Close");
            dropdownPanel.SetActive(true);
            menuAnimator.SetTrigger("Open");
        }
        else
        {
            // Trigger close animation
            menuAnimator.SetTrigger("Close");
        }
    }

    private void StartCooldown()
    {
        isOnCooldown = true;
        menuButton.interactable = false;
        Invoke("EndCooldown", cooldown);
    }

    private void EndCooldown()
    {
        isOnCooldown = false;
        menuButton.interactable = true;
    }

    // Called by animation event at the end of ItemCLOSED.anim
    public void OnCloseComplete()
    {
        if (!isOpen) // Double check we should still close
        {
            dropdownPanel.SetActive(false);

            // Ensure animator is disabled when closed
            menuAnimator.enabled = false;
        }
    }
}