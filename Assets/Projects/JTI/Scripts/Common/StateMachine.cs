using System;
using UnityEngine;

namespace JTI.Scripts.Common
{
    public class StateMachine
    {
        public State CurrentState { get; private set; }

        public void StartState(State state)
        {
            if (CurrentState != null)
            {
                EndState(true);
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

        public void EndState(bool change)
        {
            if (CurrentState == null || CurrentState.IsEnded) return;

            CurrentState.End(change);
        }
    }
	public class State
    {
        public Action<bool> OnEndEvent;
        public bool IsEnded;

        public void End(bool change = false)
        {
            if (IsEnded) return;

            IsEnded = true;
            OnEnd(change);
            OnEndEvent?.Invoke(change);
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
        public virtual void OnEnd(bool a)
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