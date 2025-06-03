using UnityEngine;
using UnityEngine.UI; // Required for Button

public class HamburgerMenu : MonoBehaviour
{
	[Header("UI References")]
	public GameObject dropdownPanel;
	public Button closeBackground; // Assign a transparent panel

	private Animator menuAnimator;
	private bool isOpen;

	void Start()
	{
		menuAnimator = dropdownPanel.GetComponent<Animator>();
		dropdownPanel.SetActive(false);

		// Setup background click
		closeBackground.onClick.AddListener(CloseMenu);
		closeBackground.gameObject.SetActive(false); // Hidden by default
	}

	public void ToggleMenu()
	{
		isOpen = !isOpen;

		if (isOpen)
		{
			dropdownPanel.SetActive(true);
			menuAnimator.Play("Closed", -1, 0f);
			menuAnimator.SetTrigger("Open");
			closeBackground.gameObject.SetActive(true); // Show blocker
		}
		else
		{
			menuAnimator.SetTrigger("Close");
		}
	}

	public void OnCloseComplete()
	{
		if (!isOpen)
		{
			dropdownPanel.SetActive(false);
			closeBackground.gameObject.SetActive(false); // Hide blocker
		}
	}

	void CloseMenu()
	{
		if (isOpen) ToggleMenu();
	}
}