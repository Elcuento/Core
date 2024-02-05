using System.Linq;
using JTI.Scripts.Common;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JTI.Scripts
{
    public class Joystick : MonoBehaviour
    {
        public UnityEvent<Vector2> MoveEvent;

        public static Joystick[] Instance
        {
            get
            {
                if (_instance == null || _instance.Length == 0)
                {
                    _instance = FindObjectsOfType<Joystick>().ToArray();
                }

                return _instance;
            }
        }

        private static Joystick[] _instance;


        [SerializeField] private bool _moveClickPosition = true;
        [SerializeField] private bool _hideWhenNotUse = true;
        [SerializeField] private int _number = 0;
        [SerializeField] private CanvasGroup _alpha;
        [SerializeField] private Image _handler;
        [SerializeField] private float _range;
        [SerializeField] private float _deathZone = 10;
        [SerializeField] private ClickHandler _clickHandler;

        public Vector2 Axis { get; set; }
        public int Number => _number;

        private bool _isPressed;

        private Canvas _canvas;

        private void Start()
        {
            _canvas = gameObject.GetComponentInParent<Canvas>();

            if (_hideWhenNotUse)
            {
                _alpha.alpha = 0;
            }

            _clickHandler.OnClickDownEvent += PressDown;
            _clickHandler.OnClickEvent += Press;
            _clickHandler.OnClickUpEvent += UnPress;
        }

        private void OnDestroy()
        {
            if (_clickHandler == null)
                return;

            _clickHandler.OnClickDownEvent -= PressDown;
            _clickHandler.OnClickEvent -= Press;
            _clickHandler.OnClickUpEvent -= UnPress;
        }

        private void PressDown()
        {
            if (_isPressed)
                return;

            if (_moveClickPosition)
            {
                if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                {
                    transform.position = _clickHandler.Position;
                }
                else
                {
                    transform.position = Utils.ScreenToUISpace(_canvas,
                        _clickHandler.Position);
                }
            }
            if (_hideWhenNotUse)
            {
                _alpha.alpha = 1;
            }

            _isPressed = true;

        }

        private void Press()
        {
            if (!_isPressed)
                return;

            Vector2 pos;

            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                pos = _clickHandler.Position;
            }
            else
            {
                pos = Utils.ScreenToUISpace(_canvas,
                    _clickHandler.Position);
            }

            var d = Vector3.Distance((Vector2)transform.position, pos);
            var max = Mathf.Clamp(d / _range, 0, 1);
            var dir = (pos - (Vector2)transform.position).normalized;
            _handler.transform.localPosition = max * _handler.transform.InverseTransformVector(_range * dir);

            var dist = Vector3.Distance(_handler.transform.position, transform.position);

            if (dist > _deathZone)
                Axis = dir * dist / _range;
            else Axis = Vector2.zero;

            Axis = dir.normalized * max;

            MoveEvent?.Invoke(Axis);
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            if (_hideWhenNotUse)
            {
                _alpha.alpha = 0;
            }

            Axis = Vector3.zero;
            _isPressed = false;
            _handler.transform.localPosition = Vector3.zero;

            MoveEvent?.Invoke(Axis);
        }
    }
}
