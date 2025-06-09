using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public PomodoroTimer timerReference;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeSettings()
    {
        // Initialize only if not set
        if (!PlayerPrefs.HasKey("StudyDuration"))
        {
            PlayerPrefs.SetInt("StudyDuration", 25);
            PlayerPrefs.SetInt("ShortBreakDuration", 5);
            PlayerPrefs.SetInt("LongBreakDuration", 15);
            PlayerPrefs.SetInt("UseLongBreaks", 0);
            PlayerPrefs.SetFloat("AlarmVolume", 0.5f);
        }

        // Initialize timer state keys if missing
        if (!PlayerPrefs.HasKey("PomodoroIsStudy"))
        {
            PlayerPrefs.SetInt("PomodoroIsStudy", 1);
        }

        PlayerPrefs.Save();
    }
}