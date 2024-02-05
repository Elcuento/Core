using System;
using UnityEngine;

public class Pinch : MonoBehaviour
{
    public Action<float> OnPinchEvent;

    private void Update()
    {
        var touchZero = Input.GetTouch(0);
        var touchOne = Input.GetTouch(1);

        var touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        var touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

        var prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        var touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

        var deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;

        if (Math.Abs(prevTouchDeltaMag - touchDeltaMag) > 0.01f)
        {
            OnPinchEvent?.Invoke(deltaMagnitudeDiff);
        }
    }
}
