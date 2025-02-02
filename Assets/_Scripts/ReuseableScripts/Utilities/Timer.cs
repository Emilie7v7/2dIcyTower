using System;
using UnityEngine;

namespace _Scripts.ReuseableScripts.Utilities
{
    public class Timer
    {
        public event Action OnTimerDone;

        private float _startTime;
        private float _targetTime;
        private readonly float _duration;

        private bool _isActive;

        public Timer(float duration)
        {
            this._duration = duration;
        }

        public void StartTimer()
        {
            _startTime = Time.time;
            _targetTime = _startTime + _duration;
            _isActive = true;
        }

        public void StopTimer()
        {
            _isActive = false;
        }

        public void Tick()
        {
            if (!_isActive) return;

            if (Time.time >= _targetTime)
            {
                OnTimerDone?.Invoke();
                StopTimer();
            }
        }
    }
}
