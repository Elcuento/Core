using JTI.Scripts.Events;
using JTI.Scripts.Events.Level;

namespace JTI.Scripts.Level
{
    using UnityEngine;


    public class LevelController : MonoBehaviour
    {
        public LevelMaster Master => _master;

        protected LevelMaster _master;

        protected EventSubscriberMonoLocal<LevelEvent> _subscriber;

        protected void Awake()
        {
            OnAwake();
        }

        protected void Start()
        {
            OnStart();
        }

        public void Setup(LevelMaster master)
        {
            _master = master;

            _subscriber = new EventSubscriberMonoLocal<LevelEvent>(this, _master.EventManager);
            _subscriber.Subscribe<LevelChangeStateEvent>(LevelChangeState);
        }

        protected void OnDestroy()
        {
            _subscriber?.Destroy();

            _OnDestroy();
        }

        protected void LevelChangeState(LevelChangeStateEvent data)
        {
            OnLevelChangeState();
        }

        protected virtual void _OnDestroy()
        {

        }
        protected virtual void OnStart()
        {

        }
        protected virtual void OnAwake()
        {

        }

        protected virtual void OnLevelChangeState()
        {

        }

        public void SetPause(bool isPause)
        {
            
        }
    }
}
