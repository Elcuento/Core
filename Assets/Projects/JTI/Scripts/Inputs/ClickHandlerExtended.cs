using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static JTI.Scripts.ClickHandlerExtended;

namespace JTI.Scripts
{
    public class ClickHandlerExtended : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
    {
        public Action OnUpdateEvent;
        public Action<Touch> OnAddTouchEvent;
        public class Touch
        {
            public int Id;
            public UnityEngine.Touch TouchSource;
            public bool UnPressed;
        }

        public List<Touch> Touches { get; private set; } = new List<Touch>();


        public GraphicRaycaster _raycaster;
        PointerEventData pointerEventData;
        EventSystem eventSystem;

        private Vector3 _lastClick;

        private void OnDisable()
        {
            _isOn = false;

            foreach (var touch in Touches)
            {
                touch.UnPressed = true;
            }

            Touches.Clear();
        }

        public void ForceAddTouchClick(UnityEngine.Touch aTouch)
        {
            var exist = Touches.Find(x => x.TouchSource.fingerId == aTouch.fingerId);
            if (exist != null) return;

            var touch = new Touch
            {
                Id = aTouch.fingerId,
                TouchSource = aTouch
            };

            Touches.Add(touch);

            try
            {
                OnAddTouchEvent?.Invoke(touch);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
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

        public void ForceClick()
        {
            if (Application.isMobilePlatform)
            {
                foreach (var touch in Input.touches)
                {
                    var exist = Touches.Find(x => x.TouchSource.fingerId == touch.fingerId);

                    if (exist == null)
                    {
                        var to = new Touch
                        {
                            Id = touch.fingerId,
                            TouchSource = touch
                        };

                        Touches.Add(to);

                        try
                        {
                            OnAddTouchEvent?.Invoke(to);
                        }
                        catch (Exception e)
                        {
                            Debug.LogError(e);
                        }

                        Update();

                        break;

                    }
                }
            }
            else
            {
                if (Touches.Count == 0)
                {
                    var touch = new Touch()
                    {
                        TouchSource = new UnityEngine.Touch(),
                        Id = 0
                    };

                    Touches.Add(touch);

                    try
                    {
                        OnAddTouchEvent?.Invoke(touch);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }

                    Update();
                }
            }



        }

        private bool _updated;
        private bool _isOn;

        private void Update()
        {
            if (_updated)
                return;

            _updated = true;

            if (Application.isMobilePlatform)
            {
                if (Input.touchCount > 0)
                {
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        var t = Input.GetTouch(i);

                        Touch touch = null;

                        if (t.phase == TouchPhase.Began && (CheckClick(t.position)) && _isOn)
                        {
                            Touches.Add(touch = new Touch()
                            {
                                Id = t.fingerId,
                            });
                        }
                        else
                        {
                            touch = Touches.Find(x => x.Id == t.fingerId);
                        }

                        if (touch != null)
                        {
                            touch.Id = t.fingerId;
                            touch.TouchSource = t;
                        }
                    }
                }

                try
                {
                    OnUpdateEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    // ignored
                }


                for (var index = 0; index < Touches.Count; index++)
                {
                    if (Touches[index].TouchSource.phase == TouchPhase.Ended)
                    {
                        Touches.Remove(Touches[index]);
                        index--;
                    }
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _lastClick = Input.mousePosition;

                    if (Touches.Count != 0)
                    {
                        Touches.Clear();
                    }

                    if (CheckClick(Input.mousePosition))
                    {
                        var touch = new Touch()
                        {
                            TouchSource = new UnityEngine.Touch(),
                            Id = 0
                        };

                        touch.TouchSource.phase = TouchPhase.Began;

                        Touches.Add(touch);
                    }
                }
                else if (Touches.Count != 0)
                {
                    if (!Input.GetKeyUp(KeyCode.Mouse0) && Input.GetKey(KeyCode.Mouse0))
                    {
                        foreach (var touch in Touches)
                        {
                            touch.TouchSource.phase = TouchPhase.Moved;
                        }
                    }

                    if (Input.GetKeyUp(KeyCode.Mouse0))
                    {
                        foreach (var touch in Touches)
                        {
                            touch.TouchSource.phase = TouchPhase.Ended;
                        }
                    }
                }


                var mouseDelta = (Input.mousePosition - _lastClick).normalized;

                foreach (var touch in Touches)
                {
                    touch.TouchSource.deltaPosition = mouseDelta;
                    touch.TouchSource.position = Input.mousePosition;
                }

                try
                {
                    OnUpdateEvent?.Invoke();
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    // ignored
                }


                for (var index = 0; index < Touches.Count; index++)
                {
                    if (Touches[index].TouchSource.phase == TouchPhase.Ended)
                    {
                        Touches.Remove(Touches[index]);
                        index--;
                    }
                }
            }

            _isOn = false;
        }

        private void LateUpdate()
        {
            _updated = false;
        }

        private bool CheckClick(Vector3 pos)
        {
            pointerEventData = new PointerEventData(eventSystem) { position = pos };

            var results = new List<RaycastResult>();

            /*  var allRaycasters = FindObjectsOfType<GraphicRaycaster>(); // Bad one


              foreach (var allRaycaster in allRaycasters)
              {
                  allRaycaster.Raycast(pointerEventData, results);

                  for (var index = 0; index < results.Count; index++)
                  {
                      var result = results[index];

                      if (result.gameObject != gameObject)
                      {
                          return false;
                      }
                  }
              }
              */
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

        public void OnPointerClick(PointerEventData eventData)
        {
           
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isOn = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {

        }
    }
}
