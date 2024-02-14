using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JTI.Scripts
{
    public class ClickHandlerSimplified : MonoBehaviour//,  IPointerExitHandler, IPointerUpHandler, IPointerDownHandler
    {
        public Action OnClickEvent;

        public GraphicRaycaster _raycaster;
        PointerEventData pointerEventData;
        EventSystem eventSystem;

        private bool _click;
        private bool _forceClick;
        private void Start()
        {
            if (_raycaster == null)
            {
                _raycaster = FindObjectOfType<GraphicRaycaster>();
            }

            eventSystem = FindObjectOfType<EventSystem>();
        }

        public void ForceClick()
        {
            _forceClick = true;
            _click = true;
            Debug.Log("FORE" + Input.touchCount);
        }

        private void Update()
        {
            if (Application.isMobilePlatform)
            {
                if (Input.touchCount > 0)
                {
                    Debug.Log("LTAEA" + Input.touchCount);
                    for (var i = 0; i < Input.touchCount; i++)
                    {
                        var t = Input.GetTouch(i);

                        if (t.phase == TouchPhase.Began)
                        {
                            if (CheckClick(t.position) || _forceClick)
                            {
                                _click = true;
                            }
                            else
                            {
                                Debug.Log("False");
                                _click = false;
                                return;
                            }
                        }
                        else if (t.phase == TouchPhase.Ended)
                        {
                            Debug.Log("End");
                        }
                    }

                    Press();
                }
                else
                {
                    _forceClick = false;
                    _click = false;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    Press();
                }else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    Press();
                }
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    _click = true;
                    Press();
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

        private void Press()
        {
           // Debug.Log("Pres" + _click);
            if (_click)
            {
                OnClickEvent?.Invoke();
            }
        }
  
    }
}
