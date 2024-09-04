using System;
using System.Collections.Generic;
using System.Linq;
using Assets.JTI.Scripts.Events.Game;
using Assets.JTI.Scripts.Events.Singleton;
using JTI.Scripts.Events;
using JTI.Scripts.Events.Game;
using JTI.Scripts.GameControllers;
using JTI.Scripts.Storage;
using Unity.VisualScripting;
using UnityEngine;

namespace JTI.Scripts.Managers
{

    public class WindowController : GameControllerMono
    {

        public class WindowControllerWindowSettings 
        {

        }
        [System.Serializable]
        public class WindowSettings : GameControllerSettings
        {
            public string Folder = "Windows/";
            public int StartLayer = 100;
        }

        private readonly List<WindowControllerWindow> _windows = new List<WindowControllerWindow>();

        public static int SortingOrder { get; private set; } = 1;

        [SerializeField] private WindowSettings _settings;

        public WindowController SetSettings(WindowSettings a)
        {
            _settings = a;

            return this;
        }

        protected override  void OnAwake()
        {
            base.OnAwake();

            SortingOrder = _settings.StartLayer;

            Subscribe();
        }

        protected override void OnOnDestroy()
        {
            base.OnOnDestroy();

            UnSubscribe();
        }

        private void Subscribe()
        {
            EventManager.Instance.Subscribe<ChangeLocalizationEvent>(OnLanguageChange);
        }

        private void UnSubscribe()
        {
            if (EventManager.Instance == null)
                return;

            EventManager.Instance.Unsubscribe<ChangeLocalizationEvent>(OnLanguageChange);

        }

        private void OnLanguageChange(ChangeLocalizationEvent data)
        {
            for (var index = 0; index < _windows.Count; index++)
            {
                var windowBase = _windows[index];
                windowBase.RefreshLanguage();
            }
        }

        public WindowControllerWindow GetActiveWindow()
        {
            return _windows?.Where(x => x.gameObject.activeSelf)?.OrderByDescending(x => x.SortingOrder)
                .FirstOrDefault();
        }

        public bool IsCurrent(WindowControllerWindow window)
        {
            for (var index = 0; index < _windows.Count; index++)
            {
                var windowBase = _windows[index];
                if (windowBase == window || windowBase == null || !windowBase.gameObject.activeSelf)
                    continue;

                if (windowBase.SortingOrder >= window.SortingOrder)
                    return false;
            }

            return true;
        }

        public T Show<T>(WindowControllerWindowSettings settings = null) where T : WindowControllerWindow
        {
            if (settings == null)
                settings = new WindowControllerWindowSettings();


            SortingOrder++;

            var sortingOrder = SortingOrder;

            var window = (T)_windows.FirstOrDefault(w => w is T);
            if (window == null)
            {

                var type = typeof(T);
                var windowPrefab =
                    Resources.Load<T>($"{GameManager.Instance.ResourcesPath + _settings.Folder}{type.Name}");

                window = Instantiate(windowPrefab, transform, false);

                window.Init(this);

                window.SetSortingOrder(sortingOrder);

                _windows.Add(window);
                window.Show(settings);
            }
            else
            {
                window.transform.SetAsLastSibling();
                window.SetSortingOrder(sortingOrder);


                if (window.gameObject.activeSelf)
                    window.BecameCurrent();
                else
                {
                    window.gameObject.SetActive(true);
                    window.Show(settings);
                }
            }

            return window;
        }


        public bool IsOpen<T>() where T : WindowControllerWindow
        {
            var window = (T)_windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
            return window != null;
        }

        public bool IsClosed<T>() where T : WindowControllerWindow
        {
            var window = (T)_windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
            return window == null;
        }

        public T Get<T>() where T : WindowControllerWindow
        {
            return (T)_windows.FirstOrDefault(w => w is T && w.gameObject.activeSelf);
        }

        public List<WindowControllerWindow> GetAllWindowsWithLayerMore(int layer)
        {
            return _windows.Where(x => x.gameObject.activeSelf && x.SortingOrder > layer).ToList();
        }

        public void Close<T>(bool withDestroy = false, bool withSound = true) where T : WindowControllerWindow
        {
            var window = _windows.LastOrDefault(w => w.gameObject.activeSelf);
            if (window != null && window is T)
            {
                SortingOrder--;
            }
            else
                window = _windows.FirstOrDefault(w => w is T);

            CloseWindow(window, withDestroy, withSound);
        }

        public void Close(WindowControllerWindow windowBase, bool withDestroy = false, bool withSound = true)
        {
            var lastOpenedWindow = _windows.LastOrDefault(w => w.gameObject.activeSelf);
            if (lastOpenedWindow != null && lastOpenedWindow == windowBase)
            {
                SortingOrder--;
            }

            CloseWindow(windowBase, withDestroy, withSound);
        }

        public void Clear()
        {
            var windowsToDestroy = new List<WindowControllerWindow>();

            foreach (var window in _windows)
            {

                if (window.tag == "UiWindowAboveAll" ||
                    window.tag == "UiWindowSystem")
                    continue;

                windowsToDestroy.Add(window);
            }

            foreach (var windowToDestroy in windowsToDestroy)
            {
                _windows.Remove(windowToDestroy);
                Destroy(windowToDestroy.gameObject);
            }

            SortingOrder = _settings.StartLayer;
        }


        private void CloseWindow(WindowControllerWindow window, bool withDestroy, bool withSound = true)
        {
            if (window == null || window.IsClosing)
                return;

            window.OnClosing();

            if (withDestroy)
            {
                _windows.Remove(window);
                Destroy(window.gameObject);
            }
            else
                window.gameObject.SetActive(false);

            var lastOpenedWindow = _windows.LastOrDefault(w => w.gameObject.activeSelf && !w.IsClosing);
            if (lastOpenedWindow != null)
            {
                lastOpenedWindow.BecameCurrent();
            }

            EventManager.Instance.Publish(new WindowCloseEvent()
            {
                Window = window
            });
        }
    }
}
