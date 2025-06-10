using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DurationButton : MonoBehaviour
{
    public enum DurationType { Study, ShortBreak, LongBreak }
    public DurationType durationType;
    public int minutes;

    [Header("Selection State")]
    public Image selectionIndicator;
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetDuration);
        UpdateSelectionState();
    }

    public void SetDuration()
    {
        PlayerPrefs.SetInt($"{durationType}Selected", minutes);

        switch (durationType)
        {
            case DurationType.Study:
                PlayerPrefs.SetInt("StudyDuration", minutes);
                break;
            case DurationType.ShortBreak:
                PlayerPrefs.SetInt("ShortBreakDuration", minutes);
                break;
            case DurationType.LongBreak:
                PlayerPrefs.SetInt("LongBreakDuration", minutes);
                break;
        }
        PlayerPrefs.Save();

        // Reset timer when durations change
        if (GameManager.Instance != null && GameManager.Instance.timerReference != null)
        {
            GameManager.Instance.timerReference.LoadSettings();
            GameManager.Instance.timerReference.ResetTimer();
        }

        UpdateAllButtonsInCategory();
    }

    public void UpdateSelectionState()
    {
        if (selectionIndicator != null)
        {
            bool isSelected = PlayerPrefs.GetInt($"{durationType}Selected", -1) == minutes;
            selectionIndicator.color = isSelected ? selectedColor : normalColor;
        }
    }

    private void UpdateAllButtonsInCategory()
    {
        foreach (var button in FindObjectsOfType<DurationButton>())
        {
            if (button.durationType == durationType)
            {
                button.UpdateSelectionState();
            }
        }
    }
}