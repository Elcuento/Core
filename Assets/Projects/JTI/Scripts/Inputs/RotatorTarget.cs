using UnityEngine;

namespace JTI.Scripts
{
    public class RotatorTarget : MonoBehaviour
    {
        public Transform Target;

        [SerializeField] private float _mouseSensitive = 400;

        public Vector3 Axis { get; private set; }

        [SerializeField] private ClickHandler _clickHandler;

        private bool _isPressed;

        private void Start()
        {
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

            _isPressed = true;

        }

        private float _xRotation;
        private float _yRotation;

        private void Press()
        {
            if (!_isPressed)
                return;

            var mouseX = _clickHandler.Delta.x * _mouseSensitive * Time.deltaTime;
            var mouseY = _clickHandler.Delta.y * _mouseSensitive * Time.deltaTime;

           _xRotation -= mouseY;
           _xRotation = Mathf.Clamp(_xRotation, -90, 90f);

           _yRotation += mouseX;

           Target.rotation = (Quaternion.Euler(_xRotation, _yRotation, 0f));
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            _isPressed = false;

            Axis = new Vector3(0, 0);
        }
    }
}
