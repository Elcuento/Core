using System.Collections.Generic;
using System.Linq;
using JTI.Examples;
using JTI.Scripts.Common;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace JTI.Scripts
{
    public class Joystick : MonoBehaviour
    {
        public UnityEvent<Vector2> MoveEvent;
        public UnityEvent ReleaseEvent;
        public UnityEvent PressEvent;
        public static List<Joystick> Instance
        {
            get
            {
                for (var index = 0; index < _instance.Count; index++)
                {
                    if (_instance[index] == null)
                    {
                        _instance.Remove(_instance[index]);
                        index--;
                    }
                }

                if (_instance == null || _instance.Count == 0)
                {
                    _instance = FindObjectsOfType<Joystick>().ToList();
                }

                return _instance;
            }
        }

        private static List<Joystick> _instance;

        [SerializeField] private bool _moveClickPosition = true;
        [SerializeField] private bool _hideWhenNotUse = true;
        [SerializeField] private int _number = 0;
        [SerializeField] private CanvasGroup _alpha;
        [SerializeField] private Image _handler;
        [SerializeField] private float _range;
        [SerializeField] private float _deathZone = 10;
        [SerializeField] private ClickHandler _clickHandler;

        public bool IsUsing { get; set; }
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
                    transform.position = PositionAndRotationHelper.ScreenToUISpace(_canvas,
                        _clickHandler.Position);
                }
            }
            if (_hideWhenNotUse)
            {
                _alpha.alpha = 1;
            }
            _isPressed = true;

            PressEvent?.Invoke();

        }

        private void Press()
        {
            if (!_isPressed)
                return;

            IsUsing = true;

            Vector2 pos;

            if (_canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                pos = _clickHandler.Position;
            }
            else
            {
                pos = PositionAndRotationHelper.ScreenToUISpace(_canvas,
                    _clickHandler.Position);
            }

            var localPoint = transform.InverseTransformPoint(pos);
            localPoint.z = 0;
            var localDistance = localPoint.magnitude;
            var percent = Mathf.Clamp((localDistance - _deathZone) / (_range - _deathZone), 0, 1);
            var dir = localPoint.normalized;

            _handler.transform.localPosition = percent * _range * dir;

            Axis = dir * percent;

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

            IsUsing = false;
            Axis = Vector3.zero;
            _isPressed = false;
            _handler.transform.localPosition = Vector3.zero;

            MoveEvent?.Invoke(Axis);
            ReleaseEvent?.Invoke();
        }


    }
}
