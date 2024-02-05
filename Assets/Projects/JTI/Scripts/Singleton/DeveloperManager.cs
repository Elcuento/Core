using System;
using System.Collections.Generic;
using JTI.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class DeveloperManager : SingletonMono<DeveloperManager>
{
    public class DeveloperItem
    {
        protected GameObject _main;
        private Action _createAction;
        private Action _updateAction;

        public Settings _settings;

        public DeveloperItem(Settings s, Transform c)
        {
            _settings = s;
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
        public Text Text;
        public DeveloperButton(Settings s, Transform c, string text, Action action) : base(s, c)
        {
            Setup(() =>
            {
                var button = _main.AddComponent<Button>();
                button.image = button.gameObject.AddComponent<Image>();
                button.onClick.AddListener(() => action?.Invoke());
                button.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                var t = new GameObject("Text");
                Text = t.AddComponent<Text>();
                Text.text = text;
                Text.resizeTextForBestFit = true;
                Text.transform.SetParent(button.transform);
                Text.color = Color.black;
                Text.font = Font;
                Text.alignment = TextAnchor.MiddleCenter;


                var rec = Text.GetComponent<RectTransform>();
                rec.anchorMax = new Vector2(1, 1);
                rec.anchorMin = new Vector2(0, 0);

                rec.offsetMax = new Vector2(0, 0);
                rec.offsetMin = new Vector2(0, 0);

                if (_settings.Hide)
                {
                    _main.AddComponent<CanvasGroup>().alpha = 0;
                }

            });
        }


    }

    public class DeveloperInput : DeveloperItem
    {
        public InputField InputField;
        public DeveloperInput(Settings s, Transform c, string txt, Action<string> a) : base(s, c)
        {
            Setup(() =>
            {
                var im = _main.AddComponent<Image>();
                InputField = _main.AddComponent<InputField>();

                InputField.onValueChanged.AddListener((astr) =>
                {
                    a?.Invoke(InputField.text);
                });

                im.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                var t = new GameObject("Text").AddComponent<Text>();

                t.text = txt;
                t.resizeTextForBestFit = true;
                t.transform.SetParent(im.transform);
                t.color = Color.black;
                t.font = Font;
                t.alignment = TextAnchor.MiddleCenter; ;
                t.raycastTarget = false;
                var rec = t.GetComponent<RectTransform>();
                rec.anchorMax = new Vector2(1, 1);
                rec.anchorMin = new Vector2(0, 0);

                rec.offsetMax = new Vector2(0, 0);
                rec.offsetMin = new Vector2(0, 0);

                InputField.textComponent = t;
                InputField.text = txt;

                if (_settings.Hide)
                {
                    _main.AddComponent<CanvasGroup>().alpha = 0;
                }
            });
        }
    }

    public class DeveloperText : DeveloperItem
    {
        public Text Text;
        public DeveloperText(Settings s, Transform c, string txt, bool keepOnScreen = false) : base(s, c)
        {
            Setup(() =>
            {
                var im = _main.AddComponent<Image>();
                im.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                var t = new GameObject("Text");

                Text = t.AddComponent<Text>();

                Text.text = txt;
                Text.resizeTextForBestFit = true;
                Text.transform.SetParent(im.transform);
                Text.color = Color.black;
                Text.font = Font;
                Text.alignment = TextAnchor.MiddleCenter;
                im.raycastTarget = false;
                Text.raycastTarget = false;
                var rec = Text.GetComponent<RectTransform>();
                rec.anchorMax = new Vector2(1, 1);
                rec.anchorMin = new Vector2(0, 0);

                rec.offsetMax = new Vector2(0, 0);
                rec.offsetMin = new Vector2(0, 0);

                if (_settings.Hide)
                {
                    _main.AddComponent<CanvasGroup>().alpha = 0;
                }
            });
        }
    }
    public enum ContainerPosition
    {
        Left,
        Middle,
        Right
    }
    public class Settings
    {
        public float Height = 128;
        public float WightHeight = 256;
        public bool KeepOnScreen;
        public bool Hide;
    }

    protected static Font Font { get; private set; }

    protected virtual void CreateFont()
    {
        Font = Font.CreateDynamicFontFromOSFont("Arial", 1);
    }

    private Canvas _canvas;

    private Dictionary<ContainerPosition, Transform> _containers;
    private List<DeveloperItem> _items;

    private Vector2 _prevMousePos;
    private float _swapTime;
    private bool _show;

    protected override void OnAwaken()
    {
        base.OnAwaken();

        _items = new List<DeveloperItem>();
        _containers = new Dictionary<ContainerPosition, Transform>();

        CreateFont();
        Check();
        Show(false);

        if (EventSystem.current == null)
        {
            Debug.LogError("No EventSystem in scene! Add it first");
        }
    }


    private void Check()
    {
        if (_canvas == null)
        {
            _canvas = new GameObject("Canvas").AddComponent<Canvas>();
            _canvas.transform.SetParent(transform);
            _canvas.gameObject.AddComponent<CanvasScaler>();
            _canvas.gameObject.AddComponent<GraphicRaycaster>();
            _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            _canvas.sortingOrder = 9999;
        }

        _containers.Add(ContainerPosition.Left, CreateContainer(ContainerPosition.Left, new Vector3(0, 1), new Vector2(0, 1), new Vector2(0, 1)));
        _containers.Add(ContainerPosition.Middle, CreateContainer(ContainerPosition.Middle, new Vector3(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0.5f, 1)));
        _containers.Add(ContainerPosition.Right, CreateContainer(ContainerPosition.Right, new Vector3(1, 1), new Vector2(1, 1), new Vector2(1, 1)));

    }

    private Transform CreateContainer(Enum type, Vector3 pivot, Vector2 anchorMax, Vector3 anchorMin)
    {
        var c = new GameObject(type.ToString());
        var v = c.AddComponent<VerticalLayoutGroup>();
        c.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        c.transform.SetParent(_canvas.transform);

        v.childControlHeight = false;
        v.childControlWidth = false;

        var rect = c.GetComponent<RectTransform>();
        rect.pivot = pivot;
        rect.anchorMax = anchorMax;
        rect.anchorMin = anchorMin;
        rect.anchoredPosition = Vector3.zero;

        return c.transform;
    }

    protected DeveloperItem CreateFps()
    {
        var b = AddText("Fps : ", ContainerPosition.Middle);

        b.SetUpdate(() => { b.Text.text = "Fps : " + PerformanceManager.Instance.AverageFps; });

        return b;
    }

    protected void ShowMinimize()
    {
        OpenPage(new List<DeveloperItem>()
        {
            // AddButton("", Main, hide:true)
        }, showClose: false);
    }
    protected virtual void Main()
    {
        var c = AddText("Scene : ");

        c.SetUpdate(() =>
        {
            c.Text.text = "Scene : " + SceneManager.GetActiveScene().name;
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

    protected virtual void OpenPage(List<DeveloperItem> list, ContainerPosition pos = ContainerPosition.Left, bool showClose = true)
    {
        Clear();

        /*var fps = CreateFpsInMiddle();

        list.Add(fps);
        */
        if (showClose)
        {
            list.Add(AddButton("Back", Main));
        }

        foreach (var developerItem in list)
        {
            developerItem.Create();
        }

        if (_containers[pos].transform.childCount > 0)
        {
            _containers[pos].transform.GetChild(_containers[pos].transform.childCount - 1).SetAsFirstSibling();
        }

        _items = list;
    }

    protected DeveloperInput AddInputField(string text, ContainerPosition pos = ContainerPosition.Left, Action<string> a = null)
    {
        var b = new DeveloperInput(new Settings(), _containers[pos].transform, text, a);

        return b;
    }
    protected DeveloperButton AddButton(string text, Action a = null, ContainerPosition pos = ContainerPosition.Left)
    {
        var b = new DeveloperButton(new Settings(), _containers[pos].transform, text, a);

        return b;
    }
    protected DeveloperText AddText(string text, ContainerPosition pos = ContainerPosition.Left)
    {
        var b = new DeveloperText(new Settings(), _containers[pos].transform, text);

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
                        if (!_show)
                        {
                            ShowHide();
                            Main();
                        }
                    }
                }

            }
        }


        if (!_show)
        {
            if (Input.GetKeyUp(KeyCode.Tilde) || Input.GetKeyUp(KeyCode.Tab))
            {
                ShowHide();
                Main();
            }

            return;
        }
        else
        {
            if (_prevCursorState != CursorLockMode.Locked)
            {
                _prevCursorState = Cursor.lockState;
            }

        }

        if (Input.GetKeyUp(KeyCode.Tilde) || Input.GetKeyUp(KeyCode.Tab))
        {
            ShowHide();
        }
        else
        {
            foreach (var developerItem in _items)
            {
                developerItem.OnUpdate();
            }
        }


    }

    private CursorLockMode _prevCursorState;

    public void Show(bool a)
    {
        _show = !a;
        ShowHide();
    }
    public void ShowHide()
    {
        _show = !_show;

        if (_show)
        {
            Main();
        }
        else
        {
            ShowMinimize();
        }

        Cursor.lockState = _show ? CursorLockMode.None : _prevCursorState;
    }
}
