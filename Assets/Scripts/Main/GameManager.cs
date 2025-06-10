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
        // Duration settings
        if (!PlayerPrefs.HasKey("StudyDuration"))
        {
            PlayerPrefs.SetInt("StudyDuration", 25);
            PlayerPrefs.SetInt("ShortBreakDuration", 5);
            PlayerPrefs.SetInt("LongBreakDuration", 15);
        }
        
        // Selection states
        if (!PlayerPrefs.HasKey("StudySelected"))
        {
            PlayerPrefs.SetInt("StudySelected", 25);
        }
        if (!PlayerPrefs.HasKey("ShortBreakSelected"))
        {
            PlayerPrefs.SetInt("ShortBreakSelected", 5);
        }
        if (!PlayerPrefs.HasKey("LongBreakSelected"))
        {
            PlayerPrefs.SetInt("LongBreakSelected", 15);
        }
        
        // Other settings
        if (!PlayerPrefs.HasKey("UseLongBreaks"))
        {
            PlayerPrefs.SetInt("UseLongBreaks", 0);
        }
        if (!PlayerPrefs.HasKey("AlarmVolume"))
        {
            PlayerPrefs.SetFloat("AlarmVolume", 0.5f);
        }
        
        // Timer state
        if (!PlayerPrefs.HasKey("PomodoroIsStudy"))
        {
            PlayerPrefs.SetInt("PomodoroIsStudy", 1);
        }

        PlayerPrefs.Save();
    }
}