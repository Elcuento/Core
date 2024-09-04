using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiCameraFinder : MonoBehaviour
{
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponent<Canvas>();

        RefreshCamera();
    }
    private void RefreshCamera()
    {
        Camera worldCamera = null;

        if (_canvas.worldCamera != null)
            return;

        try
        {
            var uiCameraGo = GameObject.FindWithTag("UiCamera");

            if (uiCameraGo != null)
                worldCamera = uiCameraGo.GetComponent<Camera>();
        }
        catch (Exception e)
        {
            // ignored
        }


        if (worldCamera == null)
            worldCamera = Camera.main;


        _canvas.worldCamera = worldCamera;

        Destroy(this);
    }
}
