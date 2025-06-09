using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class PomodoroTimer : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text stateText; // NEW: Added state text reference
    public Button playPauseButton;
    public Button resetButton;
    public Image playPauseImage;
    public Sprite playSprite;
    public Sprite pauseSprite;

    // Timer settings
    private int studyDuration = 25;
    private int shortBreakDuration = 5;
    private int longBreakDuration = 15;
    private bool useLongBreaks = false;
    private float alarmVolume = 0.5f;

    private TimeSpan currentTime;
    private DateTime endTime;
    private bool isStudyPhase = true;
    private bool isRunning = false;
    private int sessionCount = 0;

    // PlayerPrefs keys
    private const string EndTimeKey = "PomodoroEndTime";
    private const string IsStudyKey = "PomodoroIsStudy";
    private const string IsRunningKey = "PomodoroIsRunning";
    private const string SessionCountKey = "PomodoroSessionCount";
    private const string CurrentTimeKey = "PomodoroCurrentTime";

    void Start()
    {
        // Register with GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.timerReference = this;
        }

        LoadSettings();
        LoadTimerState();  // Load state before initializing currentTime

        // Initialize currentTime based on loaded state
        if (!isRunning)
        {
            currentTime = isStudyPhase ?
                TimeSpan.FromMinutes(studyDuration) :
                TimeSpan.FromMinutes(shortBreakDuration);
        }

        playPauseButton.onClick.AddListener(TogglePlayPause);
        resetButton.onClick.AddListener(ResetTimer);

        UpdateTimerDisplay();  // Ensure display is updated immediately
        UpdateButtonImage();
        UpdateStateText(); // NEW: Initialize state text
    }

    void Update()
    {
        if (isRunning)
        {
            currentTime = endTime - DateTime.Now;

            if (currentTime.TotalSeconds <= 0)
            {
                CompletePhase();
            }

            UpdateTimerDisplay();
        }
    }

    void LoadSettings()
    {
        studyDuration = PlayerPrefs.GetInt("StudyDuration", 25);
        shortBreakDuration = PlayerPrefs.GetInt("ShortBreakDuration", 5);
        longBreakDuration = PlayerPrefs.GetInt("LongBreakDuration", 15);
        useLongBreaks = PlayerPrefs.GetInt("UseLongBreaks", 0) == 1;
        alarmVolume = PlayerPrefs.GetFloat("AlarmVolume", 0.5f);
    }

    void LoadTimerState()
    {
        isRunning = PlayerPrefs.GetInt(IsRunningKey, 0) == 1;
        isStudyPhase = PlayerPrefs.GetInt(IsStudyKey, 1) == 1;  // Default to study phase
        sessionCount = PlayerPrefs.GetInt(SessionCountKey, 0);

        if (isRunning)
        {
            if (PlayerPrefs.HasKey(EndTimeKey))
            {
                long endTimeBinary = Convert.ToInt64(PlayerPrefs.GetString(EndTimeKey));
                endTime = DateTime.FromBinary(endTimeBinary);
                currentTime = endTime - DateTime.Now;

                if (currentTime.TotalSeconds <= 0)
                {
                    HandleExpiredTimer();
                }
            }
            else
            {
                // Handle missing end time by resetting
                ResetTimer();
            }
        }
        else
        {
            if (PlayerPrefs.HasKey(CurrentTimeKey))
            {
                long currentTimeTicks = Convert.ToInt64(PlayerPrefs.GetString(CurrentTimeKey,
                    TimeSpan.FromMinutes(studyDuration).Ticks.ToString()));
                currentTime = TimeSpan.FromTicks(currentTimeTicks);
            }
            else
            {
                // Default to study duration
                currentTime = TimeSpan.FromMinutes(studyDuration);
                isStudyPhase = true;
            }
        }
    }

    void HandleExpiredTimer()
    {
        isStudyPhase = !isStudyPhase;
        sessionCount = isStudyPhase ? (sessionCount + 1) : sessionCount;
        currentTime = GetNextPhaseTime();
        endTime = DateTime.Now.Add(currentTime);
        PlayAlarm();
        SaveTimerState();
        UpdateStateText(); // NEW: Update state after phase change
    }

    void TogglePlayPause()
    {
        if (isRunning)
        {
            isRunning = false; // Pause
        }
        else
        {
            isRunning = true; // Start/Resume
            endTime = DateTime.Now.Add(currentTime);
        }

        SaveTimerState();
        UpdateButtonImage();
        UpdateTimerDisplay();  // Ensure UI updates immediately
    }

    // CHANGED TO PUBLIC
    public void ResetTimer()
    {
        isRunning = false; // Stop the timer
        isStudyPhase = true;
        sessionCount = 0;
        currentTime = TimeSpan.FromMinutes(studyDuration);
        SaveTimerState();
        UpdateTimerDisplay();
        UpdateButtonImage();
        UpdateStateText(); // NEW: Update state after reset
    }

    void CompletePhase()
    {
        isStudyPhase = !isStudyPhase;
        sessionCount = isStudyPhase ? (sessionCount + 1) : sessionCount;
        currentTime = GetNextPhaseTime();
        endTime = DateTime.Now.Add(currentTime);
        PlayAlarm();
        SaveTimerState();
        UpdateTimerDisplay();  // Ensure UI updates after phase change
        UpdateStateText(); // NEW: Update state after phase change
    }

    TimeSpan GetNextPhaseTime()
    {
        if (isStudyPhase)
        {
            return TimeSpan.FromMinutes(studyDuration);
        }
        else
        {
            if (useLongBreaks && sessionCount >= 3)
            {
                sessionCount = 0;
                return TimeSpan.FromMinutes(longBreakDuration);
            }
            return TimeSpan.FromMinutes(shortBreakDuration);
        }
    }

    void PlayAlarm()
    {
        // Implement your audio playback here
        Debug.Log("Alarm! Volume: " + alarmVolume);
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = $"{Math.Abs(currentTime.Minutes):D2}:{Math.Abs(currentTime.Seconds):D2}";
        }
        else
        {
            Debug.LogError("TimerText reference is missing!");
        }
    }

    // NEW: State text update method
    void UpdateStateText()
    {
        if (stateText != null)
        {
            if (isStudyPhase)
            {
                stateText.text = "Study";
                stateText.color = Color.red; // Red for study
            }
            else
            {
                // Check if this is a long break
                if (useLongBreaks && sessionCount >= 3)
                {
                    stateText.text = "Long Break";
                    stateText.color = Color.green; // Green for long break
                }
                else
                {
                    stateText.text = "Short Break";
                    stateText.color = Color.blue; // Blue for short break
                }
            }
        }
        else
        {
            Debug.LogError("StateText reference is missing!");
        }
    }

    void UpdateButtonImage()
    {
        if (playPauseImage != null)
        {
            playPauseImage.sprite = isRunning ? pauseSprite : playSprite;
        }
    }

    void SaveTimerState()
    {
        PlayerPrefs.SetInt(IsRunningKey, isRunning ? 1 : 0);
        PlayerPrefs.SetInt(IsStudyKey, isStudyPhase ? 1 : 0);
        PlayerPrefs.SetInt(SessionCountKey, sessionCount);

        if (isRunning)
        {
            PlayerPrefs.SetString(EndTimeKey, endTime.ToBinary().ToString());
            PlayerPrefs.DeleteKey(CurrentTimeKey);
        }
        else
        {
            PlayerPrefs.SetString(CurrentTimeKey, currentTime.Ticks.ToString());
            PlayerPrefs.DeleteKey(EndTimeKey);
        }

        PlayerPrefs.Save();
    }

    void OnDestroy() => SaveTimerState();
    void OnApplicationQuit() => SaveTimerState();
    void OnApplicationPause(bool pauseStatus) => SaveTimerState();
}