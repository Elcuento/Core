using System;
using System.Collections.Generic;
using UnityEngine;

namespace JTI.Scripts.UI
{
    public class UiConfig
    {

    }
    public class UiBase<B> : MonoBehaviour where B : UiConfig, new()
    {
        [SerializeField] protected List<UiBase<B>> _uiList;
        [SerializeField] protected GameObject _root;

        protected UiBase<B> _ui;

        private void Awake()
        {
            foreach (var lobbyUiBase in _uiList)
            {
                if (lobbyUiBase == null)
                {
                    Debug.LogError("Dont exist ui " + this.GetType());
                    continue;
                }
                lobbyUiBase.Init(this);
            }

            OnAwake();
        }

        public virtual void OnClickOpen(int i)
        {
            if (_uiList.Count <= i) return;

            for (var index = 0; index < _uiList.Count; index++)
            {
                _uiList[index].Show(i == index);
            }
        }

        public void Init(UiBase<B> ui)
        {
            _ui = ui;
            OnInit();
        }

        protected virtual void Configure(B data)
        {

        }
        public void ShowUi<T>()
        {
            Configure(new B());

            // Debug.Log("Show " + typeof(T));
            foreach (var a in _uiList)
            {
                a.Show(a is T);
            }
        }
        public void ShowUi<T>(B data)
        {
            Configure(data);

            // Debug.Log("Show " + typeof(T));
            foreach (var a in _uiList)
            {
                a.Show(a is T);
            }
        }
        public T GetUi<T>() where T : UiBase<B>
        {
            foreach (var a in _uiList)
            {
                if (a is T) return (T)a;
            }

            return null;
        }


        protected virtual void Start()
        {

        }
        protected virtual void OnDestroy()
        {

        }

        public void Show(bool b)
        {
            _root.gameObject.SetActive(b);

            if (b)
            {
                OnShow();
            }
            else OnHide();
        }

        protected virtual void OnInit()
        {

        }
        protected virtual void OnAwake()
        {

        }
        public virtual void Activate()
        {
            foreach (var lobbyUiBase in _uiList)
            {
                lobbyUiBase.Activate();
            }

        }
        public virtual void OnHide()
        {

        }
        public virtual void OnShow()
        {

        }
    }

}

