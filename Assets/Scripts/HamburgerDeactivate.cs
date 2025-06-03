using UnityEngine;

public class HamburgerDeactivate: MonoBehaviour
{
	[SerializeField] private HamburgerMenu _menuController;

	// Called by the animation event
	public void ForwardCloseCompleteEvent()
	{
		if (_menuController != null)
		{
			_menuController.OnCloseComplete();
		}
		else
		{
			Debug.LogError("Menu Controller not assigned!", this);
		}
	}
}