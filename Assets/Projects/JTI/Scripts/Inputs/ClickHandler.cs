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
        
        public GraphicRaycaster _raycaster;

        public Vector2 Position { get; protected set; }
        public Vector2 Delta { get; protected set; }

        protected bool _click;
        private Vector3 _lastClick;
        private PointerEventData pointerEventData;
        private EventSystem eventSystem;
        protected Touch _touch;
        
        protected virtual void OnDisable()
        {
            PressUp();
        }

        protected virtual void OnEnable()
        {
            
        }
        
        private void Start()
        {
            if (_raycaster == null)
            {
                _raycaster = gameObject.GetComponentInParent<GraphicRaycaster>()
                             ?? FindObjectOfType<GraphicRaycaster>();
            }

            eventSystem = EventSystem.current;
        }

        protected virtual void Update()
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
                                Delta = t.deltaPosition;
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

                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    PressUp();
                }

                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Press();
                }
               
            }

        }

        protected bool CheckClick(Vector3 pos)
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

        protected void Press(Touch t)
        {
            if (_click)
            {
                try
                {
                    OnClickEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
             
            }
        }
        protected void Press()
        {
            if (_click)
            {
                try
                {
                    OnClickEvent?.Invoke();
                }
                catch (Exception e)
                {
                   Debug.LogError(e);
                }
             
            }
        }

        protected void PressUp(Touch t)
        {
            if (_click)
            {
                _click = false;

                try
                {
                    OnClickUpEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
             
            }
        }
        protected void PressUp()
        {
            if (_click)
            {
                _click = false;

                try
                {
                    OnClickUpEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
           
            }
        }

        protected void PressDown(Touch t)
        {
            _touch = t;
            _click = true;

            try
            {
                OnClickDownEvent?.Invoke();
            }
            catch (Exception e)
            {
               Debug.LogError(e);
            }
    
        }

        protected void PressDown()
        {
            _click = true;

            try
            {
                OnClickDownEvent?.Invoke();
            }
            catch (Exception e)
            {
               Debug.LogError(e);
            }
          
        }
    }
}
