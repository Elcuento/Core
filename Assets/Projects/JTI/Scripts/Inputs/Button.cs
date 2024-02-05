using UnityEngine;
using UnityEngine.Events;

namespace JTI.Scripts{
    public class Button : MonoBehaviour
    {
        [SerializeField] private UnityEvent _eventPress;
        [SerializeField] private UnityEvent _eventPressUp;
        [SerializeField] private UnityEvent _eventPressDown;

        [SerializeField] private ClickHandler _clickHandler;

        private bool _isPressed;

        public void Start()
        {
            _clickHandler.OnClickDownEvent += PressDown;
            _clickHandler.OnClickEvent += Press;
            _clickHandler.OnClickUpEvent += UnPress;
        }
        public void OnDestroy()
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
            _eventPressDown.Invoke();
        }

        private void Press()
        {
            if (!_isPressed)
                return;

            _eventPress.Invoke();
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            _isPressed = false;

            _eventPressUp.Invoke();
        }

    }
}
