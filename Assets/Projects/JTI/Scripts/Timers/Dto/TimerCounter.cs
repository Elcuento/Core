using System.Collections.Generic;

namespace JTI.Scripts.Timers.Dto
{
    public class TimerCounter
    {
        public List<Timer> Timers;

        public TimerCounter()
        {
            Timers = new List<Timer>();
        }
        public static TimerCounter Convert(Core.TimerCounter timer)
        {
            var list = new List<Timer>();

            foreach (var timerTimer in timer.Timers)
            {
                list.Add(Timer.Convert(timerTimer));

            }

            return new TimerCounter
            {
                Timers = list
            };
        }
    }
}
