using UnityEngine;
using UnityEngine.Events;

namespace JTI.Scripts
{
    public class Swipe : MonoBehaviour
    {
        [SerializeField] private UnityEvent<Vector2> _swipeEvent;

        [SerializeField] private ClickHandler _clickHandler;
        [SerializeField] private float _swipeRange;

        private Vector2 _pressPlace;
        private bool _isPressed;

        public void Start()
        {
            _clickHandler.OnClickDownEvent += PressDown;
            _clickHandler.OnClickUpEvent += UnPress;
        }
        public void OnDestroy()
        {
            if (_clickHandler == null)
                return;

            _clickHandler.OnClickDownEvent -= PressDown;
            _clickHandler.OnClickUpEvent -= UnPress;
        }

        private void PressDown()
        {
            if (_isPressed)
                return;

            _isPressed = true;

            _pressPlace = _clickHandler.Position;
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            _isPressed = false;

            if (Vector3.Distance(_pressPlace, _clickHandler.Position) > _swipeRange)
            {
                var delta = (_clickHandler.Position - _pressPlace).normalized;

                _swipeEvent.Invoke(delta);
            }
        }
    }
}
