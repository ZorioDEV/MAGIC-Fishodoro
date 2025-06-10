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
    public Button skipButton; // NEW: Skip button
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
        skipButton.onClick.AddListener(SkipToNextPhase); // NEW: Skip button listener

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

    public void LoadSettings()
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
        UpdateStateText(); // Update state after phase change
        currentTime = GetNextPhaseTime();
        endTime = DateTime.Now.Add(currentTime);
        PlayAlarm();
        SaveTimerState();
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
        sessionCount = 1;
        currentTime = TimeSpan.FromMinutes(studyDuration);
        UpdateStateText(); // Update state after phase change
        SaveTimerState();
        UpdateTimerDisplay();
        UpdateButtonImage();
    }

    // NEW: Skip to next phase functionality
    public void SkipToNextPhase()
    {
        isRunning = false; // Pause after skip
        isStudyPhase = !isStudyPhase;
        sessionCount = isStudyPhase ? (sessionCount + 1) : sessionCount;
        UpdateStateText(); // Update state after phase change
        currentTime = GetNextPhaseTime();
        SaveTimerState();
        UpdateTimerDisplay();
        UpdateButtonImage();
    }

    void CompletePhase()
    {
        isStudyPhase = !isStudyPhase;
        sessionCount = isStudyPhase ? (sessionCount + 1) : sessionCount;
        UpdateStateText(); // Update state after phase change
        currentTime = GetNextPhaseTime();
        endTime = DateTime.Now.Add(currentTime);
        PlayAlarm();
        SaveTimerState();
        UpdateTimerDisplay();  // Ensure UI updates after phase change
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
        // TODO: Implement your audio playback here
    }

    void UpdateTimerDisplay()
    {
        if (timerText != null)
        {
            timerText.text = $"{Math.Abs((int)currentTime.TotalMinutes):D2}:{Math.Abs(currentTime.Seconds):D2}";
        }
    }

    // NEW: State text update method
    // TODO: ADD TEXT LOCALIZATION
    void UpdateStateText()
    {
        if (stateText != null)
        {
            if (isStudyPhase)
            {
                stateText.text = "Study";
            }
            else
            {
                // Check if this is a long break
                if (useLongBreaks && sessionCount >= 3)
                {
                    stateText.text = "Long Break";
                }
                else
                {
                    stateText.text = "Short Break";
                }
            }
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