using UnityEngine;
using UnityEngine.UI;


public class HamburgerMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject dropdownPanel;
    public GameObject menuBlocker;
    public Image buttonImage;
    public Sprite menuClosedSprite;
    public Sprite menuOpenSprite;

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
        menuButton = GetComponent<Button>();

        // Initialize menu blocker
        if (menuBlocker != null)
        {
            menuBlocker.SetActive(false);
            Button blockerButton = menuBlocker.GetComponent<Button>();
            blockerButton.onClick.AddListener(() => { if (isOpen) ToggleMenu();});
        }

        menuAnimator.enabled = false;
        dropdownPanel.SetActive(false);

        isOnCooldown = false;

        // Set initial button image
        if (buttonImage != null && menuClosedSprite != null)
        {
            buttonImage.sprite = menuClosedSprite;
        }
    }

    public void ToggleMenu()
    {
        if (isOnCooldown) return;
        StartCooldown();

        isOpen = !isOpen;

        // Always enable animator before playing animations
        menuAnimator.enabled = true;

        // Swap image
        if (buttonImage != null)
        {
            buttonImage.sprite = isOpen ? menuOpenSprite : menuClosedSprite;
        }

        if (isOpen)
        {
            // Reset state before activation
            menuAnimator.ResetTrigger("Close");
            dropdownPanel.SetActive(true);
            menuAnimator.SetTrigger("Open");

            // Activate blocker
            if (menuBlocker != null) menuBlocker.SetActive(true);
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

            // Deactivate blocker
            if (menuBlocker != null) menuBlocker.SetActive(false);
        }
    }
}