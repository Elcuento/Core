using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JTI.Scripts
{
    public class ClickHandler : MonoBehaviour//,  IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Action OnClickDownEvent;
        public Action OnClickEvent;
        public Action OnClickUpEvent;

        public Vector2 Position { get; private set; }
        public Vector2 Delta { get; private set; }

        private bool _click;
        private Vector3 _lastClick;

        public GraphicRaycaster _raycaster;
        PointerEventData pointerEventData;
        EventSystem eventSystem;

        private Touch _touch;

        private void Start()
        {
            if (_raycaster == null)
            {
                _raycaster = FindObjectOfType<GraphicRaycaster>();
            }

            eventSystem = FindObjectOfType<EventSystem>();
        }

        private void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount > 0)
                {
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        var t = Input.GetTouch(i);

                        if (t.phase == TouchPhase.Began && !_click)
                        {
                            if (CheckClick(t.position))
                            {
                                Delta = (t.position - Position).normalized;
                                Position = t.position;
                                PressDown(t);
                            }
                        }

                        if (!_click) continue;
                        if (_touch.fingerId != t.fingerId) continue;

                        _touch = t;

                        Delta = (t.position - Position).normalized;
                        Position = t.position;

                        if (t.phase == TouchPhase.Ended)
                        {
                            PressUp(t);
                        }
                        else if (t.phase == TouchPhase.Canceled)
                        {
                            PressUp(t);
                        }
                        else if (t.phase == TouchPhase.Stationary || t.phase == TouchPhase.Moved)
                        {
                            Press(t);
                        }
                    }
                }
            }
            else
            {
                Delta = ((Vector2)Input.mousePosition - Position).normalized;
                Position = Input.mousePosition;

               if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (CheckClick(Position))
                    {

                        PressDown();
                    }
                }


                if (!_click) return;

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Press();
                }
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    PressUp();
                }
            }

        }

        private bool CheckClick(Vector3 pos)
        {
            pointerEventData = new PointerEventData(eventSystem) { position = pos };

            var results = new List<RaycastResult>();

            _raycaster.Raycast(pointerEventData, results);

            for (var index = 0; index < results.Count; index++)
            {
                var result = results[index];

                if (result.gameObject == gameObject && index == 0)
                {
                    return true;
                }
            }

            return false;
        }

        private void Press(Touch t)
        {
            if (_click)
            {
                OnClickEvent?.Invoke();
            }
        }
        private void Press()
        {
            if (_click)
            {
                OnClickEvent?.Invoke();
            }
        }

        private void PressUp(Touch t)
        {
            if (_click)
            {
                _click = false;
                OnClickUpEvent?.Invoke();
            }
        }
        private void PressUp()
        {
            if (_click)
            {
                _click = false;
                OnClickUpEvent?.Invoke();
            }
        }

        private void PressDown(Touch t)
        {
            _touch = t;
            _click = true;
            OnClickDownEvent?.Invoke();
        }

        private void PressDown()
        {
            _click = true;
            OnClickDownEvent?.Invoke();
        }
    }
}
