using System.Collections.Generic;

namespace Assets.JTI.Scripts.Events.Game
{
    public class ChangeSoundSettingsEvent : GameEvent
    {
        public Dictionary<int, float> GroupOnChangeVolume;

    }
}