namespace JTI.Scripts.Timers.Dto
{
    public class Timer
    {
        public string Type;
        public long Start;
        public int Period;
        public int Capacity;
        public int MaxCapacity;

        public static Timer Convert(JTI.Scripts.Timers.Core.Timer timer)
        {
            return new Timer
            {
                Type = timer.Type,
                Start = timer.Start,
                Period = timer.Period,
                Capacity = timer.CalculateCurrentCapacity(true),
                MaxCapacity = timer.MaxCapacity
            };
        }
    }
}
