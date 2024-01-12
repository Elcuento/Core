using UnityEngine;
using UnityEngine.Events;

namespace Assets.Game.Scripts.Scenes.Game.Mono.Characters
{
    public class Rotator : MonoBehaviour
    {
        public UnityEvent<Vector2> RotateEvent;

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

   /*     private float _xRotation;
        private float _yRotation;*/

        private void Press()
        {
            if (!_isPressed)
                return;


            Axis = new Vector2(_clickHandler.Delta.x, _clickHandler.Delta.y);

            RotateEvent?.Invoke(new Vector2(_clickHandler.Delta.x, _clickHandler.Delta.y));

            Debug.Log("A");
          /* _xRotation -= mouseY;
           _xRotation = Mathf.Clamp(_xRotation, -90, 90f);

           _yRotation += mouseX;
            */
            //  Target.RotateCamera(Quaternion.Euler(_xRotation, _yRotation, 0f));
        }

        private void UnPress()
        {
            if (!_isPressed)
                return;

            _isPressed = false;

            Axis = new Vector3(0, 0);

            RotateEvent?.Invoke(new Vector2(0, 0));
        }
    }
}
