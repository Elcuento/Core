using Assets.Game;
using JTI.Scripts.Managers;
using UnityEngine;

namespace JTI.Scripts.Timers.Core
{
    public class Timer
    {
        public string Type { get; private set; }
        public long Start { get; private set; }
        public int Period { get; private set; }
        public int MaxCapacity { get; private set; }
        public int CurrentCapacity { get; private set; }

        public int CalculateCurrentCapacity(bool noSave = false)
        {
            Period = Period == 0 ? Period = 1 : Period;

            if (Start + Period < TimeManager.Instance.NowUnixSeconds)
            {
                var left = TimeManager.Instance.NowUnixSeconds - Start;
                var i = 0;

                while (left > Period)
                {
                    i++;
                    left -= Period;
                }

                Start = TimeManager.Instance.NowUnixSeconds - left;
                CurrentCapacity += i;
            }

            CurrentCapacity = Mathf.Clamp(CurrentCapacity, 0, MaxCapacity);
            
            if(!noSave) Save();

            return CurrentCapacity;

        }

        public bool Subtract()
        {
            CalculateCurrentCapacity();

            var cur = CurrentCapacity;

            if (cur <= 0)
            {
                return false;
            }

            if (cur >= MaxCapacity)
            {
                Start = TimeManager.Instance.NowUnixSeconds;
            }

            CurrentCapacity--;

            Save();
            return true;
        }

        public long TimeLeft
        {
            get
            {
                var res = (Start + Period) - TimeManager.Instance.NowUnixSeconds;
                res = res <= 0 ? 0 : res;
                return res;
            }
        }

        public bool IsTimeEnd => TimeLeft <= 0;
        public bool IsFull => MaxCapacity <= CalculateCurrentCapacity();

        public Timer(Dto.Timer t)
        {
            Type = t.Type;
            Start = t.Start;
            CurrentCapacity = t.Capacity;
            MaxCapacity = t.MaxCapacity;
            Period = t.Period;
        }

        public void Restart()
        {
            Start = TimeManager.Instance.NowUnixSeconds;
        }
        public Timer(string type, int period, int maxCapacity)
        {
            Type = type;
            Start = TimeManager.Instance.NowUnixSeconds;
            MaxCapacity = maxCapacity;
            Period = period;
        }

        public Timer(string type, int period, int maxCapacity, int currentCapacity)
        {
            Type = type;
            Start = TimeManager.Instance.NowUnixSeconds;
            MaxCapacity = maxCapacity;
            CurrentCapacity = currentCapacity;
            Period = period;
        }

        public void Save()
        {
          //  GameMaster.Instance.TimerService.Save();
        }

        public void AddCapacity(int a)
        {
            CurrentCapacity = Mathf.Clamp(CurrentCapacity + a, 0, MaxCapacity);
        }

        public void SetPeriod(int period) => Period = period;
        public void SetMaxCapacity(int capacity) => MaxCapacity = capacity;
        public void SetStartCapacity(int capacity) => CurrentCapacity = capacity;
    }
}
