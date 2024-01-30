using System;
using UnityEngine;

namespace JTI.Scripts.GameControllers
{
    public class TimeControllerView : GameControllerWrapper
    {
        public Action OnTickUpdate;

        public void Update()
        {
            OnTickUpdate?.Invoke();
        }
    }
}

