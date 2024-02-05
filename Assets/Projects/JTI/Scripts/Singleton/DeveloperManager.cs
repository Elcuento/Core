using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JTI.Scripts.Managers
{
    public class DeveloperManager : SingletonMono<DeveloperManager>
    {
        public Action OnUpdateEvent;

        public class Settings
        {
            public float Height = 128;
            public float WightHeight = 256;
        }

        public static Font Font;

        public virtual Settings CurrentSettings
        {
            get { return new Settings(); }
        }

        protected virtual void CreateFont()
        {
            Font = Font.CreateDynamicFontFromOSFont("Arial", 1);
        }

        public class DeveloperItem
        {
            protected GameObject _main;
            private Action _createAction;
            private Action _updateAction;

            public DeveloperItem(Settings s, Transform c)
            {
                _main = new GameObject(GetType().Name);
                _main.transform.SetParent(c);
            }

            public void SetUpdate(Action ac)
            {
                _updateAction = ac;
            }

            public virtual void OnUpdate()
            {
                _updateAction?.Invoke();
            }

            public virtual void Create()
            {
                _createAction?.Invoke();
                _createAction = null;
            }

            protected void Setup(Action createAction)
            {
                _createAction = createAction;
            }

            public void Destroy()
            {
                UnityEngine.Object.Destroy(_main);
            }
        }

        public class DeveloperButton : DeveloperItem
        {
            public TextMeshProUGUI Text;

            public DeveloperButton(Settings s, Transform c, string text, Action action) : base(s, c)
            {
                Setup(() =>
                {
                    var button = _main.AddComponent<UnityEngine.UI.Button>();
                    button.image = button.AddComponent<Image>();
                    button.onClick.AddListener(() => action?.Invoke());
                    button.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                    var t = new GameObject("Text").AddComponent<Text>();
                    t.text = text;
                    t.resizeTextForBestFit = true;
                    t.transform.SetParent(button.transform);
                    t.color = Color.black;
                    t.font = Font;
                    t.alignment = TextAnchor.MiddleCenter;
                    var rec = t.GetComponent<RectTransform>();
                    rec.anchorMax = new Vector2(1, 1);
                    rec.anchorMin = new Vector2(0, 0);

                    rec.offsetMax = new Vector2(0, 0);
                    rec.offsetMin = new Vector2(0, 0);

                });
            }


        }

        public class DeveloperText : DeveloperItem
        {
            public Text Text;

            public DeveloperText(Settings s, Transform c, string txt) : base(s, c)
            {
                Setup(() =>
                {
                    var im = _main.AddComponent<Image>();
                    im.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                    var t = new GameObject("Text").AddComponent<Text>();

                    Text = t;

                    t.text = txt;
                    t.resizeTextForBestFit = true;
                    t.transform.SetParent(im.transform);
                    t.color = Color.black;
                    t.font = Font;
                    t.alignment = TextAnchor.MiddleCenter;
                    var rec = t.GetComponent<RectTransform>();
                    rec.anchorMax = new Vector2(1, 1);
                    rec.anchorMin = new Vector2(0, 0);

                    rec.offsetMax = new Vector2(0, 0);
                    rec.offsetMin = new Vector2(0, 0);
                });
            }
        }

        public class DeveloperInput : DeveloperItem
        {
            public TMP_InputField Input;

            public DeveloperInput(Settings s, Transform c) : base(s, c)
            {
            }
        }

        [SerializeField] private Canvas _canvas;
        [SerializeField] private GameObject _container;

        private List<DeveloperItem> _items;
        private Vector2 _prevMousePos;
        private float _swapTime;

        protected override void OnAwaken()
        {
            base.OnAwaken();

            _items = new List<DeveloperItem>();

            CreateFont();
            Check();
            Main();

            _container.gameObject.SetActive(false);
        }


        private void Check()
        {
            if (_canvas == null)
            {
                _canvas = new GameObject("Canvas").AddComponent<Canvas>();
                _canvas.transform.SetParent(transform);
                _canvas.AddComponent<CanvasScaler>();
                _canvas.AddComponent<GraphicRaycaster>();
                _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }

            if (_container == null)
            {
                _container = new GameObject("Container");
                var v = _container.AddComponent<VerticalLayoutGroup>();
                _container.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
                _container.transform.SetParent(_canvas.transform);

                v.childControlHeight = false;
                v.childControlWidth = false;

                var rect = _container.GetComponent<RectTransform>();
                rect.pivot = new Vector2(0, 1);
                rect.anchorMax = new Vector2(0, 1);
                rect.anchorMin = new Vector2(0, 1);
                rect.anchoredPosition = Vector3.zero;
            }
        }

      
        protected virtual void Main()
        {
            var b = AddText("Fps : ");
            var c = AddText("Scene : ");

            b.SetUpdate(() => { b.Text.text = "Fps : " + PerformanceManager.Instance.AverageFps; });

            c.SetUpdate(() => { c.Text.text = "Scene : " + SceneManager.GetActiveScene().name; });

            OpenPage(new List<DeveloperItem>()
            {
                b,
                c,
                AddButton("Currency", Currency),
                AddButton("Currency2", Currency)
            });
        }

        protected virtual void Currency()
        {
            OpenPage(new List<DeveloperItem>()
            {
                AddButton("Add", () => { }),
                AddButton("Remove", () => { })
            });
        }

        public void Clear()
        {
            foreach (var developerItem in _items)
            {
                developerItem.Destroy();
            }

            _items.Clear();
        }

        protected virtual void OpenPage(List<DeveloperItem> list)
        {
            Clear();

            list.Add(AddButton("Back", Main));

            foreach (var developerItem in list)
            {
                developerItem.Create();
            }

            _container.transform.GetChild(_container.transform.childCount - 1).SetAsFirstSibling();

            _items = list;
        }


        private DeveloperButton AddButton(string text, Action a)
        {
            var b = new DeveloperButton(CurrentSettings, _container.transform, text, a);

            return b;
        }

        private DeveloperText AddText(string text)
        {
            var b = new DeveloperText(CurrentSettings, _container.transform, text);

            return b;
        }

        private void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _prevMousePos = Input.mousePosition;
                    _swapTime = Time.time + 0.3f;

                }

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    var dir = (Vector2)Input.mousePosition - _prevMousePos;
                    if (dir.magnitude > Screen.height / 2f && _swapTime > Time.time)
                    {
                        if (Vector2.Angle(dir.normalized, Vector2.down) < 15)
                        {
                            if (!_container.gameObject.activeSelf)
                            {
                                ShowHide();
                                Main();
                            }
                        }
                    }

                }
            }
            else
            {
                if (Input.GetKeyUp(KeyCode.Tilde))
                {
                    ShowHide();
                }
            }

           

            for (var index = 0; index < _items.Count; index++)
            {
                _items[index].OnUpdate();
            }
        }

        public void ShowHide()
        {
            if (!_container.gameObject.activeSelf)
            {
                Main();
            }

            _container.gameObject.SetActive(!_container.gameObject.activeSelf);

            if (_container.gameObject.activeSelf)
            {
                _container.gameObject.SetActive(true);
            }
        }
    }
}