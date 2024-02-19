using System;
using UnityEngine;
using UnityEngine.Events;

namespace JTI.Scripts
{
    public class Rotator : MonoBehaviour
    {
        [SerializeField] private float _deathZone = 0;
        public Action<Vector2> OnRotateEvent { get; private set; }
        public Vector3 Axis { get; private set; }

        [SerializeField] private ClickHandler _clickHandler;

        private bool _isPressed;
        private Vector3 _lastClickPlace;

        private void Start()
        {
            _clickHandler.OnClickDownEvent += PressDown;
            _clickHandler.OnClickEvent += Press;
            _clickHandler.OnClickUpEvent += UnPress;

            _lastClickPlace = _clickHandler.Position;
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

            _isPressed = true;

        }

        private void Press()
        {
            if (!_isPressed)
                return;

            if (Vector3.Distance(_clickHandler.Position, _lastClickPlace) < _deathZone)
            {
                Axis = new Vector3();
                return;
            }

            Axis = new Vector2(_clickHandler.Delta.x, _clickHandler.Delta.y);

            OnRotateEvent?.Invoke(new Vector2(_clickHandler.Delta.x, _clickHandler.Delta.y));

            _lastClickPlace = _clickHandler.Position;
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            _isPressed = false;

            Axis = new Vector3(0, 0);

            OnRotateEvent?.Invoke(new Vector2(0, 0));
        }
    }
}