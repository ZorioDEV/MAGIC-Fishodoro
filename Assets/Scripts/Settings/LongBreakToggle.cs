using UnityEngine;
using UnityEngine.UI;

public class LongBreakToggle : MonoBehaviour
{
    [Header("UI Images")]
    public Image buttonImage;
    public Sprite onSprite;
    public Sprite offSprite;

    private Button button;
    private bool isOn;

    void Start()
    {
        button = GetComponent<Button>();

        // Load saved state or default to off
        isOn = PlayerPrefs.GetInt("UseLongBreaks", 0) == 1;
        UpdateButtonImage();

        // Add listener for click
        button.onClick.AddListener(ToggleState);
    }

    private void ToggleState()
    {
        isOn = !isOn;
        PlayerPrefs.SetInt("UseLongBreaks", isOn ? 1 : 0);
        PlayerPrefs.Save();

        UpdateButtonImage();
    }

    private void UpdateButtonImage()
    {
        if (buttonImage != null)
        {
            buttonImage.sprite = isOn ? onSprite : offSprite;
        }
    }
}