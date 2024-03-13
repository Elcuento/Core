using System;
using System.Collections.Generic;
using JTI.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using Cursor = UnityEngine.Cursor;
using Image = UnityEngine.UI.Image;

public class DeveloperManager : SingletonMono<DeveloperManager>
{
    public class DeveloperItem
    {
        protected GameObject _main;
        private Action _createAction;

        public Settings _settings;

        public DeveloperItem(Settings s, Transform c)
        {
            _settings = s;
            _main = new GameObject(GetType().Name);
            _main.transform.SetParent(c);
        }

        public virtual void OnUpdate()
        {

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
        private Action<DeveloperButton> _updateAction;
        private Button _button;
        public Text Text;

        public DeveloperButton(Settings s, Transform c, string text, Action action, Action<DeveloperButton> onUpdate = null) : base(s, c)
        {
            Setup(() =>
            {
                _updateAction = onUpdate;

                _button = _main.AddComponent<Button>();
                _button.image = _button.gameObject.AddComponent<Image>();
                _button.onClick.AddListener(() => action?.Invoke());
                _button.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                var t = new GameObject("Text");
                Text = t.AddComponent<Text>();
                Text.text = text;
                Text.resizeTextForBestFit = true;
                Text.transform.SetParent(_button.transform);
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
            _updateAction = onUpdate;
        }

        public override void OnUpdate()
        {
            _updateAction?.Invoke(this);
        }
    }
    public class DeveloperInputWithText : DeveloperItem
    {
        public InputField InputField;
        public Text Text;
        private Action<DeveloperInputWithText> _onUpdate;
        public DeveloperInputWithText(Settings s, Transform c, string txt, string inputText, Action<string> a, Action<DeveloperInputWithText> onUpdate) : base(s, c)
        {
            Setup(() =>
            {
                var mainFrame = _main.AddComponent<Image>();
                mainFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);
                var g = mainFrame.gameObject.AddComponent<HorizontalLayoutGroup>();
                g.childControlWidth = false;
                mainFrame.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                void CreateText()
                {
                    var textContainer = new GameObject("textContainer");
                    textContainer.transform.SetParent(_main.transform);

                    var im = textContainer.AddComponent<Image>();
                    textContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

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
                }
                void CreateInput()
                {
                    _onUpdate = onUpdate;

                    var inputTextContainer = new GameObject("inputTextContainer");
                    inputTextContainer.transform.SetParent(_main.transform);

                    var im = inputTextContainer.AddComponent<Image>();
                    InputField = inputTextContainer.AddComponent<InputField>();

                    InputField.onValueChanged.AddListener((astr) =>
                    {
                        a?.Invoke(InputField.text);
                    });

                    im.GetComponent<RectTransform>().sizeDelta = new Vector2(s.WightHeight, s.Height);

                    var t = new GameObject("Text").AddComponent<Text>();

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
                    InputField.text = inputText;

                    if (_settings.Hide)
                    {
                        _main.AddComponent<CanvasGroup>().alpha = 0;
                    }
                }

                CreateText();
                CreateInput();



            });
        }

        public override void OnUpdate()
        {
            _onUpdate?.Invoke(this);
        }
    }

    public class DeveloperInput : DeveloperItem
    {
        public InputField InputField;
        private Action<DeveloperInput> _onUpdate;
        public DeveloperInput(Settings s, Transform c, string txt, Action<string> a, Action<DeveloperInput> onUpdate) : base(s, c)
        {
            Setup(() =>
            {
                _onUpdate = onUpdate;

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

        public override void OnUpdate()
        {
            _onUpdate?.Invoke(this);
        }
    }

    public class DeveloperText : DeveloperItem
    {
        public Text Text;

        private Action<DeveloperText> _onUpdate;

        public DeveloperText(Settings s, Transform c, string txt, bool keepOnScreen = false, Action<DeveloperText> onUpdate = null) : base(s, c)
        {
            Setup(() =>
            {
                _onUpdate = onUpdate;

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

        public override void OnUpdate()
        {
            _onUpdate?.Invoke(this);
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

    protected Settings _settings;

    protected virtual void CreateFont()
    {
        Font = Font.CreateDynamicFontFromOSFont("Arial", 1);
    }

    private Canvas _canvas;

    private Dictionary<ContainerPosition, ScrollRect> _containers;
    private List<DeveloperItem> _items;

    private Vector2 _prevMousePos;
    private float _swapTime;
    private bool _show;

    protected override void OnAwaken()
    {
        base.OnAwaken();

        _items = new List<DeveloperItem>();
        _containers = new Dictionary<ContainerPosition, ScrollRect>();
        _settings = new Settings();

        CreateFont();
        Check();
        ShowHide(false);

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

    private ScrollRect CreateContainer(Enum type, Vector3 pivot, Vector2 anchorMax, Vector3 anchorMin)
    {
        var c = new GameObject(type.ToString());
        c.transform.SetParent(_canvas.transform);
        var view = c.gameObject.AddComponent<ScrollRect>();
        var viewRect = view.gameObject.GetComponent<RectTransform>();
        viewRect.sizeDelta = new Vector2(Screen.width / 3f, Screen.height);
        view.horizontal = false;

        var viewPort = new GameObject("ViewPort");
        var viewPortRect = viewPort.AddComponent<RectTransform>();
        viewPort.transform.SetParent(view.transform);
        viewPortRect.anchorMax = new Vector2(1, 1);
        viewPortRect.anchorMin = new Vector2(0, 0);
        viewPortRect.offsetMax = new Vector2(0, 0);
        viewPortRect.offsetMin = new Vector2(0, 0);
        viewPortRect.anchoredPosition = Vector3.zero;
        view.viewport = viewPortRect;

        var container = new GameObject("Container");
        container.transform.SetParent(viewPort.transform);
        view.content = container.AddComponent<RectTransform>();
        view.content.anchorMax = new Vector2(1, 1);
        view.content.anchorMin = new Vector2(0, 0);
        view.content.offsetMax = new Vector2(0, 0);
        view.content.offsetMin = new Vector2(0, 0);
        view.content.pivot = new Vector2(0, 1);

        var v = container.AddComponent<VerticalLayoutGroup>();
        v.gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        v.childControlHeight = false;
        v.childControlWidth = false;

        var rect = c.GetComponent<RectTransform>();
        rect.pivot = pivot;
        rect.anchorMax = anchorMax;
        rect.anchorMin = anchorMin;
        rect.anchoredPosition = Vector3.zero;

        return view;
    }

    /*   protected DeveloperItem CreateFps()
       {
           var b = AddText("Fps : ", ContainerPosition.Middle);

           b.SetUpdate(() => { b.Text.text = "Fps : " + PerformanceManager.Instance.AverageFps; });

           return b;
       }*/

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

        OpenPage(new List<DeveloperItem>()
       {
           AddText("Welcome")
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

        if (list.Count > 0)
        {
            list.Add(AddButton("Exit", ShowHideSwitch));
            list.Add(AddText(Application.version));
        }


        foreach (var developerItem in list)
        {
            developerItem.Create();
        }

        if (_containers[pos].content.transform.childCount > 0)
        {
            _containers[pos].content.transform.GetChild(_containers[pos].transform.childCount - 1).SetAsFirstSibling();
        }

        _items = list;
    }

    protected DeveloperInput AddInputField(string text, ContainerPosition pos = ContainerPosition.Left, Action<string> a = null, Action<DeveloperInput> onUpdate = null)
    {
        var b = new DeveloperInput(_settings, _containers[pos].content, text, a, onUpdate);

        return b;
    }
    protected DeveloperInputWithText AddTextWithInputField(object text, object inputText = null, ContainerPosition pos = ContainerPosition.Left, Action<string> a = null, Action<DeveloperInputWithText> onUpdate = null)
    {
        var b = new DeveloperInputWithText(_settings, _containers[pos].content, text.ToString(), inputText?.ToString() ?? "", a, onUpdate);

        return b;
    }
    protected DeveloperButton AddButton(string text, Action a = null, ContainerPosition pos = ContainerPosition.Left, Action<DeveloperButton> onUpdate = null)
    {
        var b = new DeveloperButton(_settings, _containers[pos].content, text, a, onUpdate: onUpdate);

        return b;
    }
    protected DeveloperText AddText(string text, ContainerPosition pos = ContainerPosition.Left, Action<DeveloperText> onUpdate = null)
    {
        var b = new DeveloperText(_settings, _containers[pos].content, text, onUpdate: onUpdate);

        return b;
    }


    public void SetSettings(Settings settings)
    {
        _settings = settings;
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

            if (Input.touchCount == 5)
            {
                ShowHide(true);
                Main();
            }
            /*   if (Input.GetKeyUp(KeyCode.Mouse0))
               {
                   var dir = (Vector2)Input.mousePosition - _prevMousePos;
                   if (dir.magnitude < Screen.height / 2f && _swapTime > Time.time)
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

               }*/
        }
        else
        {
            if (Input.GetKeyUp(KeyCode.Tilde) || Input.GetKeyUp(KeyCode.Tab))
            {
                if (!_show)
                {
                    if (_prevCursorState != CursorLockMode.Locked)
                    {
                        _prevCursorState = Cursor.lockState;
                    }
                }

                ShowHideSwitch();
                Main();
            }


        }


        if (!_show) return;

        foreach (var developerItem in _items)
        {
            developerItem.OnUpdate();
        }


    }

    private CursorLockMode _prevCursorState;

    public void ShowHideSwitch()
    {
        var s = !_show;

        ShowHide(s);
    }

    public void ShowHide(bool s)
    {
        _show = s;

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
