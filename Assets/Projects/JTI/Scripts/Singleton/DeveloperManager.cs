using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using JTI.Scripts.Common;
using JTI.Scripts.Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;
using ColorUtility = UnityEngine.ColorUtility;
using Cursor = UnityEngine.Cursor;
using Debug = UnityEngine.Debug;
using Image = UnityEngine.UI.Image;
using Text = UnityEngine.UI.Text;
public class DeveloperManager : SingletonMono<DeveloperManager>
{
    public class DeveloperItem
    {
        protected RectTransform _main;
        private Action _createAction;

        public Settings _settingsGlobal;
        public SettingsElement _settingsElement;

        public DeveloperItem(Settings sGlobal, SettingsElement sElement, Transform c)
        {
            _settingsGlobal = sGlobal;
            _settingsElement = sElement;

            _main = new GameObject(GetType().Name).AddComponent<RectTransform>();
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
            UnityEngine.Object.Destroy(_main.gameObject);

            OnDestroy();
        }

        protected virtual void OnDestroy()
        {

        }
    }
    public class DeveloperButton : DeveloperItem
    {
        private Action<DeveloperButton> _updateAction;
        private Button _button;
        public Text Text;

        public DeveloperButton(Settings s, SettingsElement l, Transform c, string text, Action action, Action<DeveloperButton> onUpdate = null) : base(s, l, c)
        {
            Setup(() =>
            {
                _updateAction = onUpdate;

                _button = _main.gameObject.AddComponent<Button>();
                _button.image = _button.gameObject.AddComponent<Image>();
                _button.onClick.AddListener(() => action?.Invoke());
                _button.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);

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

                if (_settingsElement.Hide)
                {
                    _main.gameObject.AddComponent<CanvasGroup>().alpha = 0;
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
        public DeveloperInputWithText(Settings s, SettingsElement l, Transform c, string txt, string inputText, Action<string> a, Action<DeveloperInputWithText> onUpdate) : base(s, l, c)
        {
            Setup(() =>
            {
                var mainFrame = _main.gameObject.AddComponent<Image>();
                mainFrame.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);
                var g = mainFrame.gameObject.AddComponent<HorizontalLayoutGroup>();
                g.childControlWidth = false;
                mainFrame.gameObject.AddComponent<ContentSizeFitter>().horizontalFit = ContentSizeFitter.FitMode.PreferredSize;

                void CreateText()
                {
                    var textContainer = new GameObject("textContainer");
                    textContainer.transform.SetParent(_main.transform);

                    var im = textContainer.AddComponent<Image>();
                    textContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);

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

                    if (_settingsElement.Hide)
                    {
                        _main.gameObject.AddComponent<CanvasGroup>().alpha = 0;
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

                    im.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);

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

                    if (_settingsElement.Hide)
                    {
                        _main.gameObject.AddComponent<CanvasGroup>().alpha = 0;
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
        public DeveloperInput(Settings s, SettingsElement l, Transform c, string txt, Action<string> a, Action<DeveloperInput> onUpdate) : base(s, l, c)
        {
            Setup(() =>
            {
                _onUpdate = onUpdate;

                var im = _main.gameObject.AddComponent<Image>();
                InputField = _main.gameObject.AddComponent<InputField>();

                InputField.onValueChanged.AddListener((st) =>
                {
                    a?.Invoke(InputField.text);
                });

                im.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);

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

                if (_settingsElement.Hide)
                {
                    _main.gameObject.AddComponent<CanvasGroup>().alpha = 0;
                }
            });
        }

        public override void OnUpdate()
        {
            _onUpdate?.Invoke(this);
        }
    }
    public class DeveloperConsole : DeveloperItem
    {
        public List<MessageContainer> Messages;
        public ScrollRect ScrollRect;
        private Action<DeveloperConsole> _onUpdate;
        public DeveloperConsole(Settings s, SettingsElement l, Transform c, string txt, Action<DeveloperConsole> onUpdate) : base(s, l, c)
        {

            Setup(() =>
            {
                var containerRect = c.GetComponent<RectTransform>();

                _main.pivot = new Vector2(0, 1f);
                _main.sizeDelta = new Vector2(l.ContainerWidth ? containerRect.rect.width : l.Wight,
                    l.ContainerHeight ? Mathf.Abs(containerRect.sizeDelta.y) : l.Height);

                ScrollRect CreateRect()
                {
                    var o = new GameObject("ScrollRect");
                    o.transform.SetParent(_main.transform);
                    var view = o.gameObject.AddComponent<ScrollRect>();
                    var viewRect = view.gameObject.GetComponent<RectTransform>();

                    viewRect.sizeDelta = _main.sizeDelta;

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

                    v.childAlignment = TextAnchor.UpperCenter;
                    v.childControlHeight = true;
                    v.childControlWidth = true;

                    var rect = o.GetComponent<RectTransform>();
                    rect.pivot = new Vector3(0.5f, 1);
                    rect.anchorMax = new Vector3(0.5f, 1);
                    rect.anchorMin = new Vector3(0.5f, 1);
                    rect.anchoredPosition = Vector3.zero;

                    return view;
                }

                ScrollRect = CreateRect();

                var m = new List<ConsoleMessage>(Instance.ConsoleMessages);

                for (var index = 0; index < m.Count; index++)
                {
                    var instanceConsoleMessage = m[index];

                    OnGetMessage(instanceConsoleMessage);
                }

                Instance.OnGetNewConsoleMessageEvent += OnGetMessage;

            });



        }

        protected override void OnDestroy()
        {
            Instance.OnGetNewConsoleMessageEvent -= OnGetMessage;
        }

        public class MessageContainer : MonoBehaviour
        {
            public Text Text;
            public ConsoleMessage Message;
            public bool IsOpen;

            public static MessageContainer CreateItem(ConsoleMessage m)
            {
                var mO = new GameObject("Message").AddComponent<MessageContainer>();
                var txt = CreateDefaultText(mO.gameObject);
                var but = mO.gameObject.AddComponent<Button>();
                but.onClick.AddListener(() => mO.OpenSwitch());
                mO.Message = m;
                mO.Text = txt;
                mO.Text.fontSize = 26;
                mO.Text.raycastTarget = true;
                mO.Text.resizeTextForBestFit = false;

                mO.Open(false);

                return mO;
            }

            public void OpenSwitch()
            {
                Open(!IsOpen);
            }
            public void Open(bool a)
            {
                IsOpen = a;
                Text.text = FormatText(a ? Message.FullText : Message.label.Length > 0 ? Message.label : Message.text.Length > 0 ?
                                        Message.text.Substring(0, Mathf.Clamp(Message.text.Length, 0, 100)) : "No text", Message.typ);
            }

            string FormatText(string text, LogType type, bool noColor = false)
            {

                if (noColor)
                {
                    return $"[{DateTime.Now}]{text}";
                }
                else
                {
                    var color = type == LogType.Error ? Color.red :
                        type == LogType.Exception || type == LogType.Warning || type == LogType.Assert ? Color.yellow : Color.white;

                    var colorHTML = ColorUtility.ToHtmlStringRGB(color);

                    return $"<color=#{colorHTML}>[{DateTime.Now}]{text}</color>";
                }
            }

            public string GetFull()
            {
                return FormatText(Message.FullText, Message.typ, true);
            }
        }

        private void OnGetMessage(ConsoleMessage m)
        {
            var mO = MessageContainer.CreateItem(m);
            mO.transform.SetParent(ScrollRect.content);
            mO.transform.SetAsFirstSibling();
        }
        public override void OnUpdate()
        {
            _onUpdate?.Invoke(this);
        }

        public string CopyText()
        {
            var str = "";
            foreach (Transform tr in ScrollRect.content)
            {
                var mc = tr.gameObject.GetComponent<MessageContainer>();
                str += mc.GetFull() + "\n";
            }

            return str;
        }

        public void ClearText()
        {
            Instance.ConsoleMessages.Clear();
            ScrollRect.content.ClearChildObjects();
        }
    }
    public class DeveloperText : DeveloperItem
    {
        public Text Text;

        private Action<DeveloperText> _onUpdate;

        public DeveloperText(Settings s, SettingsElement l, Transform c, string txt, bool keepOnScreen = false, Action<DeveloperText> onUpdate = null) : base(s, l, c)
        {
            Setup(() =>
            {
                _onUpdate = onUpdate;

                var im = _main.gameObject.AddComponent<Image>();
                im.GetComponent<RectTransform>().sizeDelta = new Vector2(l.Wight, l.Height);

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

                if (_settingsElement.Hide)
                {
                    _main.gameObject.AddComponent<CanvasGroup>().alpha = 0;
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
        public bool AlwaysOnScreen;
        public bool Hide;
        public float Height;
    }
    public class SettingsElement
    {
        public float Height;
        public float Wight;
        public bool ContainerWidth;
        public bool ContainerHeight;
        public bool Hide;

        public static SettingsElement Default()
        {
            return new SettingsElement();
        }
        public SettingsElement()
        {
            Height = 128;
            Wight = 256;
            ContainerWidth = false;
            ContainerHeight = false;
            Hide = false;
        }
    }

    public class ConsoleMessage
    {
        public string label;
        public string text;
        public LogType typ;

        public string FullText => label + "\n" + text;
    }

    [SerializeField] private bool _showOnStart;

    public Action<ConsoleMessage> OnGetNewConsoleMessageEvent;
    public List<ConsoleMessage> ConsoleMessages { get; private set; }
    public bool IsLogsEnable { get; private set; } = true;
    public bool IsEnable { get; private set; } = true;
    protected static Font Font { get; private set; }

    protected Settings _settings;

    private Canvas _canvas;

    private Dictionary<ContainerPosition, ScrollRect> _containers;
    private List<DeveloperItem> _items;
    private bool _show;

    private long _logBufferSize = 3600000;
    private long _currentLogBufferSize;
    private string _logsFolder;
    protected virtual void CreateFont()
    {
        Font = Font.CreateDynamicFontFromOSFont("Arial", 1);
    }
    protected override void OnAwaken()
    {
        base.OnAwaken();

        ConsoleMessages = new List<ConsoleMessage>();
        _items = new List<DeveloperItem>();
        _containers = new Dictionary<ContainerPosition, ScrollRect>();
        _settings = new Settings();

        CreateFont();
        Check();
        ShowHide(_showOnStart);

        if (EventSystem.current == null)
        {
            Debug.Log("No EventSystem in scene! Add it first");
        }

        Application.logMessageReceived += OnGetMessage;

        var logText = "";

        try
        {
            if (!Directory.Exists(Application.persistentDataPath + _logsFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + _logsFolder);
            }

            logText = File.ReadAllText(Application.persistentDataPath + _logsFolder + "/Logs.txt");
            _currentLogBufferSize = logText.Length;
        }
        catch (Exception e)
        {
            if (logText.Length > 36000)
            {
                File.Delete(Application.persistentDataPath + _logsFolder + "/Logs.txt");
            }
        }

    }

    public void SetLogEnable(bool a)
    {
        IsLogsEnable = a;

        if (!IsLogsEnable)
        {
            Application.logMessageReceived -= OnGetMessage;
        }
        else
        {
            Application.logMessageReceived -= OnGetMessage;
            Application.logMessageReceived += OnGetMessage;
        }
    }
    public void SetLogBufferSize(long logBufferSize)
    {
        _logBufferSize = logBufferSize;
    }

    private void OnGetMessage(string label, string text, LogType type)
    {
        if (!IsLogsEnable)
            return;

        var mes = new ConsoleMessage()
        {
            label = label,
            text = text,
            typ = type
        };

        _currentLogBufferSize += (label + text).Length;

        ConsoleMessages.Add(mes);

        OnGetNewConsoleMessageEvent?.Invoke(mes);

        try
        {
            if (_currentLogBufferSize > _logBufferSize)
            {
                File.Delete(Application.persistentDataPath + _logsFolder + "/Logs.txt");
                _currentLogBufferSize = 0;
            }

            File.AppendAllText(Application.persistentDataPath + _logsFolder + "/Logs.txt", "\n" + $"[{DateTime.Now}][{type}][{label}]\n{text}");
        }
        catch (Exception e)
        {
            // ignored
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

        var back = view.content.gameObject.AddComponent<Image>();

        back.raycastTarget = false;
        var backC = Color.black;
        back.color = new Color(backC.r, backC.g, backC.b, 0.4f);

        var v = container.AddComponent<VerticalLayoutGroup>();
        var csF = v.gameObject.AddComponent<ContentSizeFitter>();
        csF.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        v.spacing = 5;
        v.childControlHeight = false;
        v.childControlWidth = false;

        var rect = c.GetComponent<RectTransform>();
        rect.pivot = pivot;
        rect.anchorMax = anchorMax;
        rect.anchorMin = anchorMin;
        rect.anchoredPosition = Vector3.zero;

        return view;
    }

    public static Button CreateDefaultButton(GameObject tr)
    {

        var button = tr.gameObject.AddComponent<Button>();
        button.image = button.gameObject.AddComponent<Image>();

        var txt = tr.gameObject.AddComponent<Text>();
        txt.resizeTextForBestFit = true;
        txt.transform.SetParent(button.transform);
        txt.color = Color.black;
        txt.font = Font;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.raycastTarget = false;
        txt.color = Color.white;
        var rec = txt.GetComponent<RectTransform>();
        rec.anchorMax = new Vector2(1, 1);
        rec.anchorMin = new Vector2(0, 0);

        rec.offsetMax = new Vector2(0, 0);
        rec.offsetMin = new Vector2(0, 0);

        return button;
    }
    public static Text CreateDefaultText(GameObject tr)
    {
        var txt = tr.gameObject.AddComponent<Text>();
        txt.resizeTextForBestFit = true;
        txt.transform.SetParent(tr.transform);
        txt.color = Color.black;
        txt.font = Font;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.raycastTarget = false;
        txt.color = Color.white;
        var rec = txt.GetComponent<RectTransform>();
        rec.anchorMax = new Vector2(1, 1);
        rec.anchorMin = new Vector2(0, 0);

        rec.offsetMax = new Vector2(0, 0);
        rec.offsetMin = new Vector2(0, 0);

        return txt;
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
        }, main: false);
    }

    private void ShowUtils()
    {
        OpenPage(new List<DeveloperItem>()
        {
            AddText("FPS", onUpdate: (f) =>
            {
                f.Text.text = "FPS" + PerformanceManager.Instance.AverageFps.ToString();
            } ),
            AddButton("Console", ShowConsole),
            AddText("Ver." + Application.version)
        });
    }
    private void ShowConsole()
    {
        var с = AddConsole("Console", pos: ContainerPosition.Middle);

        OpenPage(new List<DeveloperItem>()
        {
            с,
            AddButton("Copy", ()=>
            {
                GUIUtility.systemCopyBuffer = с.CopyText();
            },pos:ContainerPosition.Left),
            AddButton("Clear", ()=>
            {
                с.ClearText();
            },pos:ContainerPosition.Left),

        });
    }

    private void Main()
    {
        OpenPage(MainPage(), main: false);
    }


    protected virtual List<DeveloperItem> MainPage()
    {
        return new List<DeveloperItem>()
        {
            AddText("Main Page"),
        };
    }


    public void Clear()
    {
        foreach (var developerItem in _items)
        {
            developerItem.Destroy();
        }
        _items.Clear();
    }

    protected virtual void OpenPage(List<DeveloperItem> list, ContainerPosition pos = ContainerPosition.Left, bool main = true)
    {
        Clear();

        /*var fps 

        list.Add(fps);
        */
        if (main)
        {
            list.Add(AddButton("Back", Main));
        }

        if (list.Count > 0)
        {
            list.Add(AddButton("Utils", ShowUtils));
            list.Add(AddButton("Exit", ShowHideSwitch));
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

    protected DeveloperInput AddInputField(string text, SettingsElement el = null, ContainerPosition pos = ContainerPosition.Left, Action<string> a = null, Action<DeveloperInput> onUpdate = null)
    {
        el ??= SettingsElement.Default();

        var b = new DeveloperInput(_settings, el, _containers[pos].content, text, a, onUpdate);

        return b;
    }
    protected DeveloperInputWithText AddTextWithInputField(object text, SettingsElement el = null, object inputText = null, ContainerPosition pos = ContainerPosition.Left, Action<string> a = null, Action<DeveloperInputWithText> onUpdate = null)
    {
        el ??= SettingsElement.Default();

        var b = new DeveloperInputWithText(_settings, el, _containers[pos].content, text.ToString(), inputText?.ToString() ?? "", a, onUpdate);

        return b;
    }
    protected DeveloperConsole AddConsole(string text, SettingsElement el = null, ContainerPosition pos = ContainerPosition.Left, Action<DeveloperConsole> onUpdate = null)
    {
        if (el == null)
        {
            el = new SettingsElement();
            el.ContainerHeight = true;
            el.ContainerWidth = true;
        }

        var b = new DeveloperConsole(_settings, el, _containers[pos].content, text, onUpdate: onUpdate);

        return b;
    }
    protected DeveloperButton AddButton(string text, Action a = null, SettingsElement el = null, ContainerPosition pos = ContainerPosition.Left, Action<DeveloperButton> onUpdate = null)
    {
        el ??= SettingsElement.Default();

        var b = new DeveloperButton(_settings, el, _containers[pos].content, text, a, onUpdate: onUpdate);

        return b;
    }
    protected DeveloperText AddText(string text, SettingsElement el = null, ContainerPosition pos = ContainerPosition.Left, Action<DeveloperText> onUpdate = null)
    {
        el ??= SettingsElement.Default();

        var b = new DeveloperText(_settings, el, _containers[pos].content, text, onUpdate: onUpdate);

        return b;
    }


    public void SetSettings(Settings settings)
    {
        _settings = settings;
    }

    private void Update()
    {
        if (!IsEnable)
            return;

        if (Application.isMobilePlatform)
        {
            if (Input.touchCount == 5)
            {
                ShowHide(true);
                Main();
            }
        }
        else
        {

            if (Input.GetKeyUp(KeyCode.Tilde) || Input.GetKeyUp(KeyCode.Tab))
            {
                ShowHideSwitch();
            }
        }


        if (!_show) return;

        foreach (var developerItem in _items)
        {
            developerItem.OnUpdate();
        }


    }

    public void ShowHideSwitch()
    {
        var s = !_show;

        ShowHide(s);
    }

    public void ShowHide(bool s)
    {
        if (!IsEnable)
            return;

        _show = s;

        if (_show)
        {
            Main();
        }
        else
        {
            ShowMinimize();
        }

        if (_show)
        {
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void SetLogsFolder(string configId)
    {
        _logsFolder = configId;
    }
    public void Enable(bool en)
    {
        IsEnable = en;
        //  Debug.Log("Debug set " + GameManager.Instance.IsDebug);
    }


}
