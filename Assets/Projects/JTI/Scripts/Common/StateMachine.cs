using System;

namespace JTI.Scripts.Common
{
    public class StateMachine
    {
        public State CurrentState { get; private set; }

        public void StartState(State state)
        {
            if (CurrentState != null)
            {
                EndState();
            }

            CurrentState = state;
            CurrentState.Start();
        }

        public void StopState()
        {
			if (CurrentState == null || CurrentState.IsEnded) return;

            CurrentState.Stop();
        }

        public void UpdateState()
        {
            if (CurrentState == null || CurrentState.IsEnded) return;

            CurrentState.Update();
		}

        public void EndState()
        {
            if (CurrentState == null || CurrentState.IsEnded) return;

            CurrentState.End();
        }
    }
	public class State
    {
        public Action OnEndEvent;
        public bool IsEnded;

        public void End()
        {
            if (IsEnded) return;

            IsEnded = true;
            OnEndEvent?.Invoke();
            OnEnd();
        }

        public void Start()
        {
            OnStart();
        }

        public void Update()
        {
            OnUpdate();
        }

        public void Stop()
        {
            OnStop();
        }

		public virtual void OnUpdate()
        {

        }
        public virtual void OnEnd()
        {

        }

        public virtual void OnStart()
        {

        }
        public virtual void OnStop()
        {
           
        }


	}
}