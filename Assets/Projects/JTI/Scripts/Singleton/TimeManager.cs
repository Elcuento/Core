using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using DG.Tweening;
using JTI.Scripts.Timers.Core;
using UnityEngine;

namespace JTI.Scripts.Managers
{
    public class TimeManager : SingletonMonoSimple<TimeManager>
    {
        public const int CriticalFps = 10;

        public DateTime Now { get; private set; }

        public long NowUnixSeconds { get; private set; }
        public long NowUnixSecondsExtra { get; private set; }

        private float _startTimeScale;

        private float _secondsTimerValue;

        private float _unscaledSecondsTimerValue;

        private long _networkTime;

        private float _slowMoTimer;

        private float _physicDelta;

        private float _currentTimeScale;

        public void SetPause(bool pause)
        {
            Time.timeScale = pause ? 0 : _startTimeScale;
            Time.fixedDeltaTime = _physicDelta;

            _currentTimeScale = Time.timeScale;

            _slowMoTimer = 0;
        }

        protected override void OnAwaken()
        {
            _currentTimeScale = 1;

            Update();

            _startTimeScale = Time.timeScale;

            _physicDelta = Time.fixedDeltaTime;
        }


        public void SlowMo(float time, float timeScale)
        {
            if (Time.timeScale <= 0)
                return;

            // AudioManager.Instance.PlayOneShot("slow_mo");

            if (timeScale > 0)
            {
                _slowMoTimer = time;

                Time.timeScale = timeScale;
                Time.fixedDeltaTime = _physicDelta * timeScale;
            }
        }

        public void AddExtraNowUnixSecondsTime(long a)
        {
            NowUnixSecondsExtra += a;
        }

        private void OnApplicationPause(bool isPause)
        {
            if (!isPause)
                Now = DateTime.UtcNow;
        }
        public DateTimeOffset ToDateTimeOffset(DateTime dateTime)
        {
            return dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime
                ? DateTimeOffset.MinValue
                : new DateTimeOffset(dateTime);
        }

        private void Update()
        {
            Time.timeScale = _currentTimeScale;

            if (_unscaledSecondsTimerValue <= 0)
            {
                var timestamp = DateTime.UtcNow.Ticks;
                Now = new DateTime(timestamp, DateTimeKind.Utc);

                NowUnixSeconds = ToDateTimeOffset(Now).ToUnixTimeSeconds() + NowUnixSecondsExtra;
                // EventManager.Instance.Publish(new UnscaledSecondsTickEvent());
                _unscaledSecondsTimerValue = 1f;
            }
            else
                _unscaledSecondsTimerValue -= Time.unscaledDeltaTime;

            if (_secondsTimerValue <= 0)
            {
                //   EventManager.Instance.Publish(new SecondsTickEvent());
                _secondsTimerValue = 1f;
            }
            else
            {
                _secondsTimerValue -= Time.deltaTime;
            }

            if (_slowMoTimer > 0)
            {
                _slowMoTimer -= Time.unscaledDeltaTime;

                if (_slowMoTimer <= 0)
                {
                    Time.timeScale = 1;
                    Time.fixedDeltaTime = _physicDelta;
                }
            }
        }

        public static DateTime GetNetTime()
        {
            var myHttpWebRequest = (HttpWebRequest)WebRequest.Create("http://www.microsoft.com");
            var response = myHttpWebRequest.GetResponse();
            var todaysDates = response.Headers["date"];
            return DateTime.ParseExact(todaysDates,
                "ddd, dd MMM yyyy HH:mm:ss 'GMT'",
                CultureInfo.InvariantCulture.DateTimeFormat,
                DateTimeStyles.AssumeUniversal);
        }

        public void ScaledTweenDelay(float time, Action action)
        {
            DOTween.Sequence()
                 .SetDelay(time)
                 .OnComplete(() => { action(); })
                 .SetUpdate(true);
        }

        public void UnscaledTweenDelay(float time, Action action)
        {
             DOTween.Sequence()
                 .SetDelay(time)
                 .OnComplete(() => { action(); });
        }

        public const int DefaultTimer = 60;
        public List<Timer> Timers { get; set; }

        public TimeManager()
        {
            Timers = new List<Timer>();
        }

        public void Initialize(List<Timers.Dto.Timer> timers)
        {
            Timers = new List<Timer>();

            if (timers != null)
            {
                foreach (var timer in timers)
                {
                    Timers.Add(new Timer(timer));
                }
            }

            AddGetTimer("NoAdsOffer", DefaultTimer, 1);
        }

        public Timer SetTimer(string t, int period, int capacity)
        {
            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(capacity);
                return timer;
            }

            timer = new Timer(t, period, capacity);
            Timers.Add(timer);

            Save();

            return timer;
        }

        public Timer AddGetTimer(string t, int period, int capacity)
        {

            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(capacity);
                return timer;
            }

            timer = new Timer(t, period, capacity);
            Timers.Add(timer);

            Save();

            return timer;
        }

        public Timer GetTimer(string t)
        {
            return Timers.Find(x => x.Type == t);
        }

        public void Save()
        {

        }

        public static string FormatTimeLeftNumber(long timeLeft, bool showHour = true, bool showDays = true)
        {
            var str = "";

            if (timeLeft <= 0)
            {
                str = "00:00";

                if (showHour)
                {
                    str = $"00:" + str;
                }

                if (showDays && showHour)
                {
                    str = $"00:" + str;
                }

                return str;
            }

            var date = new DateTime();
            date = date.AddSeconds(timeLeft);

            str = $"{date.Minute:D2}:{date.Second:D2}";

            if (showHour)
            {
                str = $"{date.Hour:D2}:" + str;
            }

            if (showDays && showHour)
            {
                str = $"{date.Day - 1:D2}:" + str;
            }

            return str;
        }
    }
}
