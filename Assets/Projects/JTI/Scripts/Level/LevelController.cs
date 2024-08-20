using JTI.Scripts.Events;
using JTI.Scripts.Events.Level;

namespace JTI.Scripts.Level
{
    using UnityEngine;


    public class LevelController : LevelBehaviour
    {

        protected EventSubscriberMonoLocal<LevelEvent> _subscriber;



        protected override void OnSetup()
        {
            Master.AddController(this);
            _subscriber = new EventSubscriberMonoLocal<LevelEvent>(this, Master.EventManager);


        }

        protected override void OnOnDestroy()
        {
            _subscriber?.Destroy();
        }

     
    }
}
