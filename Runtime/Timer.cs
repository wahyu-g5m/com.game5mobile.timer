using UnityEngine;
using UnityEngine.Events;

namespace Teners
{
    public class Timer : MonoBehaviour
    {
        private bool isPlayed;
        private bool isPaused;
        private bool isInitialized;
        private float currentTime;

        [Tooltip("Duration in seconds")]
        public float Duration = 1;
        public CountMode Mode = CountMode.CountDown;

        [Space]
        [HideInInspector] public UnityEvent OnPlay;
        [HideInInspector] public UnityEvent OnRestart;
        [HideInInspector] public UnityEvent OnPause;
        [HideInInspector] public UnityEvent OnResume;
        [HideInInspector] public UnityEvent<float> OnInitialize;
        [HideInInspector] public UnityEvent OnStop;
        [HideInInspector] public UnityEvent<float> OnTick;

        public bool IsRunning { get; private set; }
        public bool IsPaused => isPaused && IsRunning;

        private bool IsEnded => (Mode == CountMode.CountDown && currentTime <= 0) ||
                             (Mode == CountMode.CountUp && currentTime >= Duration);

        private void Update()
        {
            if (!IsRunning || isPaused)
            {
                return;
            }

            currentTime = Mode == CountMode.CountDown ? currentTime - Time.deltaTime : currentTime + Time.deltaTime;

            OnTick?.Invoke(currentTime);

            if (!IsEnded)
            {
                return;
            }

            Stop();
        }

        private void OnDestroy()
        {
            OnPlay.RemoveAllListeners();
            OnRestart.RemoveAllListeners();
            OnPause.RemoveAllListeners();
            OnResume.RemoveAllListeners();
            OnInitialize.RemoveAllListeners();
            OnStop.RemoveAllListeners();
            OnTick.RemoveAllListeners();
        }

        public void Initialize()
        {
            isPaused = false;
            IsRunning = false;
            currentTime = Mode == CountMode.CountDown ? Duration : 0f;
            OnInitialize.Invoke(currentTime);
            isInitialized = true;
        }

        public void Play()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            isPaused = false;
            IsRunning = true;
            isPlayed = true;
            OnPlay.Invoke();
        }

        public void Pause()
        {
            if (!isPlayed)
            {
                return;
            }

            isPaused = true;
            OnPause.Invoke();
        }

        public void Resume()
        {
            if (!isPlayed)
            {
                return;
            }

            isPaused = false;
            OnResume.Invoke();
        }

        public void Stop()
        {
            if (!isPlayed)
            {
                return;
            }

            isPaused = false;
            currentTime = Mode == CountMode.CountDown ? 0f : Duration;
            IsRunning = false;
            OnStop.Invoke();
        }

        public void Replay()
        {
            if (!isPlayed)
            {
                return;
            }

            currentTime = Mode == CountMode.CountDown ? Duration : 0f;
            isPaused = false;
            IsRunning = true;
            OnRestart.Invoke();
        }

        public enum CountMode
        {
            CountDown,
            CountUp
        }
    }
}
