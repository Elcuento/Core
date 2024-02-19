using JTI.Scripts.Level;

namespace JTI.Scripts.Events.Level
{
    public class LevelChangeStateEvent : LevelEvent
    {
        public LevelMaster.LevelState State;
        public LevelChangeStateEvent(LevelMaster master, LevelMaster.LevelState state) : base(master)
        {
            State = state;
        }
    }
}