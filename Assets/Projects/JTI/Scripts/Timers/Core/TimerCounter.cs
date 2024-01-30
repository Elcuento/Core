using System;
using System.Collections.Generic;
using Assets.Game;
using UnityEngine;

namespace JTI.Scripts.Timers.Core
{
    public class TimerCounter : MonoBehaviour
    {
        public List<Timer> Timers;

        public void Awake()
        {
            Timers = new List<Timer>();
            Load();
            DontDestroyOnLoad(gameObject);
        }

        public Timer AddGetTimer(string t, int period, int maxCapacity)
        {
            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(maxCapacity);
                return timer;
            }

            timer = new Timer(t, period, maxCapacity,0);
            Timers.Add(timer);

            Save();

            return timer;
        }

        public Timer AddGetTimer(string t, int period, int maxCapacity, int startCapacity)
        {
            var timer = Timers.Find(x => x.Type == t);

            if (timer != null)
            {
                timer.SetPeriod(period);
                timer.SetMaxCapacity(maxCapacity);
                timer.SetStartCapacity(startCapacity);
                return timer;
            }

            timer = new Timer(t, period, maxCapacity, startCapacity);
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
           // var data = Dto.TimerCounter.Convert(this);
           // GameMaster.Instance.Profile.Set(Game.Scripts.Core.Profile.ProfileVariableType.Timers, data);
        }


        public void Load()
        {
            Timers = new List<Timer>();

           /* var time = GameMaster.Instance.Profile.LoadDefault(Game.Scripts.Core.Profile.ProfileVariableType.Timers, new Dto.Profile.TimerCounter());
            if (time == null) time = new Dto.Profile.TimerCounter();


            foreach (var timer in time.Timers)
            {
                Timers.Add(new Timer(timer));
            }*/

        }
        public void Initialize()
        {
            /*AddGetTimer(""", TimerDailyMiteHuntLong,1);
          */
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
