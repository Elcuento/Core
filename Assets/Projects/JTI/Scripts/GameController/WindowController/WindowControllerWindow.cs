using System;
using System.Collections;
using System.Collections.Generic;
using Assets.JTI.Scripts.Events.Singleton;
using JTI.Examples;
using JTI.Scripts.Common;
using JTI.Scripts.Events.Game;
using JTI.Scripts.Managers;
using UnityEngine;

namespace JTI.Scripts.Managers
{

    public class WindowControllerWindow : MonoBehaviour
    {
        [SerializeField] protected GameObject _root;
        public Canvas[] Canvases { get; private set; }
        public bool IsClosing { get; private set; }
        public int SortingOrder { get; private set; }
        public bool IsReady { get; private set; }
        public bool IsCurrent => _controller.IsCurrent(this);

        public WindowController.WindowControllerWindowSettings Settings = new WindowController.WindowControllerWindowSettings();

        private int[] _canvasLayers;

        private WindowController _controller;

        protected void Awake()
        {
            Canvases = gameObject.GetComponentsInChildren<Canvas>(true);
            _canvasLayers = new int[Canvases.Length];

            for (var index = 0; index < Canvases.Length; index++)
            {
                _canvasLayers[index] = Canvases[index].sortingOrder;
            }

            OnAwake();
        }

        public void ClickClose()
        {
            _controller.Close(this);

            OnClickClose();
        }

        public void Init(WindowController con)
        {
            _controller = con;
        }

        public void Show(WindowController.WindowControllerWindowSettings settings)
        {
            Settings = settings;

            RefreshCamera();

            IsClosing = false;

            Ready();

            OnShow();

            EventManager.Instance.Publish(new WindowOpenEvent()
            {
                Window = this
            });
        }

        public void BecameCurrent()
        {
            EventManager.Instance.Publish(new WindowBecomeCurrentEvent()
            {
                Window = this
            });

            OnBecameCurrent();
        }

        public void OnClosing()
        {
            IsClosing = true;

            OnClose();
        }

        private void RefreshCamera()
        {
            Camera worldCamera = null;

            var allSet = true;

            foreach (var canvase in Canvases)
            {
                if (canvase.worldCamera == null)
                    allSet = false;
            }


            if (allSet)
                return;

            try
            {
                var uiCameraGo = GameObject.FindWithTag("UiCamera");

                if (uiCameraGo != null)
                    worldCamera = uiCameraGo.GetComponent<Camera>();
            }
            catch (Exception e)
            {
                // ignored
            }

            if (worldCamera == null)
                worldCamera = Camera.main;


            foreach (var canvase in Canvases)
            {
                canvase.worldCamera = worldCamera;
            }
        }

        protected void Ready()
        {
            IsReady = true;
            OnReady();
        }

        public Vector3 GetWorldToUi(Vector3 pos)
        {
            if (Canvases.Length > 0)
                return PositionAndRotationHelper.WorldToUISpace(Canvases[0], pos);

            return Vector3.zero;
        }

        public void RefreshLanguage()
        {
            OnRefreshLanguage();
        }

        public void SetSortingOrder(int sortingOrder)
        {
            SortingOrder = sortingOrder;

            for (var i = 0; i < _canvasLayers.Length; i++)
            {
                Canvases[i].sortingOrder = _canvasLayers[i];
                Canvases[i].sortingOrder += sortingOrder;
            }
        }

        protected virtual void OnBecameCurrent()
        {

        }

        public virtual void OnRefreshLanguage()
        {

        }

        protected virtual void OnAwake()
        {
        }

        protected virtual void OnReady()
        {
        }

        protected virtual void OnClose()
        {
        }

        protected virtual void OnClickClose()
        {
        }


        public virtual void OnShow()
        {

        }

    }
}