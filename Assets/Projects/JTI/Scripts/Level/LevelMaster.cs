using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JTI.Scripts.Events;
using JTI.Scripts.Events.Level;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace JTI.Scripts.Level
{
    public class LevelMaster : MonoBehaviour
    {
        public enum LevelState
        {
            UnStarted,
            Awake,
            Setup,
            Initialize,
            LateInitialize,
            PreStart,
            Started,
            IsEnd
        }

        public class LevelResult
        {
            public enum ResultType
            {
                UnStarted,
                Leave,
                Loose,
                Win,
                Draw
            }

            public ResultType Type;
        }

        [SerializeField] private Transform _levelField;
        [SerializeField] private List<LevelController> _controllers;
        public LevelState CurrentState { get; private set; }
        public EventManagerLocal<LevelEvent> EventManager { get; private set; }
        public List<LevelBehaviour> LevelBehaviourList { get; private set; } = new List<LevelBehaviour>();
        public Transform LevelField => _levelField;
        public bool IsPause { get; private set; }
        public LevelResult Result { get; private set; }

        private EventSubscriberMonoLocal<LevelEvent> _subscriber;

        private void Awake()
        {
            EventManager = new EventManagerLocal<LevelEvent>();
            _subscriber = new EventSubscriberMonoLocal<LevelEvent>(this, EventManager);

            CreateResult();

            OnAwake();
        }

        private void Start()
        {
            ChangeState(LevelState.Awake);

            OnStart();
        }

        private void OnDestroy()
        {
            _subscriber?.Destroy();

            OnOnDestroy();
        }
        private void SetupControllers()
        {
            for (var index = 0; index < _controllers.Count; index++)
            {
                var levelController = _controllers[index];
                if (levelController == null)
                {
                    _controllers.Remove(_controllers[index]);
                    index--;
                    continue;
                }

                _controllers[index].Setup(this);
            }
        }

        private IEnumerator StartGameCoroutine()
        {
            yield return null;
            ChangeState(LevelState.Setup);
            AddCustomControllers();
            SetupControllers();
            yield return null;
            ChangeState(LevelState.Initialize);
            yield return null;
            ChangeState(LevelState.LateInitialize);
            yield return null;
            ChangeState(LevelState.PreStart);
            yield return null;
            ChangeState(LevelState.Started);
            yield return null;
        }

        private void ChangeState(LevelState state)
        {
            if (CurrentState == LevelState.IsEnd) return;

            if (!IsCanChangeState(state)) return;

            CurrentState = state;

            switch (CurrentState)
            {
                case LevelState.IsEnd:
                    Result.Type = LevelResult.ResultType.Win;
                    StartCoroutine(EndCoroutine());
                    break;
                case LevelState.Awake:
                    StartCoroutine(StartGameCoroutine());
                    break;
            }

            _subscriber.Publish(new LevelChangeStateEvent(this, state));

            OnChangeState(state);
        }

        public bool AddController(LevelController controller)
        {
            if (_controllers.Contains(controller))
            {
                Debug.LogError("Duplicate controllres! " + SceneManager.GetActiveScene().name);
                return false;
            }

            controller.SetPause(IsPause);

            _controllers.Add(controller);

            return true;
        }

        public T GetController<T>() where T : LevelController
        {
            return (T)_controllers.FirstOrDefault(x => x is T);
        }

        public void SetPause(bool aPause)
        {
            IsPause = aPause;

            for (var index = 0; index < _controllers.Count; index++)
            {
                var levelController = _controllers[index];

                if (levelController == null)
                {
                    _controllers.Remove(_controllers[index]);
                    index--;
                    continue;
                }

                levelController.SetPause(IsPause);
            }

            for (var index = 0; index < LevelBehaviourList.Count; index++)
            {
                var levelBehaviour = LevelBehaviourList[index];

                if (levelBehaviour == null)
                {
                    LevelBehaviourList.Remove(LevelBehaviourList[index]);
                    index--;
                    continue;
                }

                levelBehaviour.SetPause(IsPause);
            }

            OnSetPause();
        }


        public void AddLevelBehaviour(LevelBehaviour levelBehaviour)
        {
            if (LevelBehaviourList.Contains(levelBehaviour)) return;

            LevelBehaviourList.Add(levelBehaviour);

            levelBehaviour.Setup(this);

            levelBehaviour.SetPause(IsPause);
        }

        public void RemoveLevelBehaviour(LevelBehaviour levelBehaviour)
        {
            LevelBehaviourList.Remove(levelBehaviour);
        }

        public T CreateController<T>() where T : LevelController
        {
            var obj = new GameObject(typeof(T).Name);
            var c = obj.AddComponent<T>();
            c.Setup(this);
            return c;
        }

        public void CompleteLevel(LevelResult result)
        {
            if (CurrentState == LevelState.IsEnd) return;

            Result = result;
            ChangeState(LevelState.IsEnd);
        }

        protected virtual IEnumerator EndCoroutine()
        {
            yield return new WaitForSecondsRealtime(0);
        }

        protected virtual void AddCustomControllers()
        {
#if UNITY_EDITOR
            /*	if (GetController<LevelController>() == null)
                {
                    CreateController<LevelController>();
                }*/
#endif

        }
        public virtual void OnSetPause()
        {

        }

        public virtual bool IsCanChangeState(LevelState state)
        {
            return true;
        }

        protected virtual void OnOnDestroy()
        {

        }
        protected virtual void OnAwake()
        {

        }
        protected virtual void OnStart()
        {

        }
        protected virtual void OnChangeState(LevelState state)
        {

        }

        protected virtual void CreateResult()
        {
            Result = new LevelResult();
        }

    }
}
