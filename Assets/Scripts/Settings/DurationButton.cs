using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class DurationButton : MonoBehaviour
{
    public enum DurationType { Study, ShortBreak, LongBreak }
    public DurationType durationType;
    public int minutes;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SetDuration);
    }

    public void SetDuration()
    {
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

        // Update timer through GameManager
        if (GameManager.Instance != null && GameManager.Instance.timerReference != null)
        {
            GameManager.Instance.timerReference.ResetTimer();
        }
    }
}