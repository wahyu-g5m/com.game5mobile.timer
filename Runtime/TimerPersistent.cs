using Five;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TimerPersistent : MonoBehaviour
{
    private bool isRunning;
    private DateTime startTime;

    [Tooltip("ID to get/set data in SecurePrefs")]
    [SerializeField] private string savedTimeStampID;
    [Tooltip("Timer duration in seconds")]
    [SerializeField, Min(1)] private long duration;

    private long SavedTimeStampSeconds
    {
        get => SecurePrefs.GetLong(savedTimeStampID);
        set => SecurePrefs.SetLong(savedTimeStampID, value);
    }

    public bool IsTimerComplete => ElapsedTime.TotalSeconds >= Duration.TotalSeconds;
    public TimeSpan Duration => TimeSpan.FromSeconds(duration);
    public TimeSpan ElapsedTime => DateTime.Now - startTime;

    public UnityEvent Ended { get; } = new();

    private void Awake()
    {
        if (string.IsNullOrEmpty(savedTimeStampID))
        {
            Debug.LogError("savedTicksID must not be empty", gameObject);
            Debug.Break();
        }

        Initialize();
    }

    private void Update()
    {
        if (isRunning)
        {
            if (IsTimerComplete)
            {
                Ended.Invoke();
                isRunning = false;
            }
        }
    }

    [ContextMenu("Run")]
    public void Run()
    {
        StartCoroutine(RunCoroutine());
    }

    [ContextMenu("ResetTimer")]
    public void ResetTimer()
    {
        startTime = DateTime.Now;
        SavedTimeStampSeconds = Utils.ConvertDateTimeToUnixTimeStamp(startTime);
    }

    public void SetSavedTicksID(string id)
    {
        savedTimeStampID = id;
    }

    public void SetDuration(TimeSpan timeSpan)
    {
        duration = (long)timeSpan.TotalSeconds;
    }

    private IEnumerator RunCoroutine()
    {
        yield return WaitFor.SecurePrefsInitialized();

        if (!SecurePrefs.HasKey(savedTimeStampID))
        {
            ResetTimer();
        }

        isRunning = true;
    }

    private void Initialize()
    {
        StartCoroutine(InitializeCoroutine());
    }

    private IEnumerator InitializeCoroutine()
    {
        yield return WaitFor.SecurePrefsInitialized();

        if (SecurePrefs.HasKey(savedTimeStampID))
        {
            startTime = Utils.ConvertUnixTimeStampToDateTime(SavedTimeStampSeconds);
        }
        else
        {
            startTime = DateTime.Now;
        }
    }
}