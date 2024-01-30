using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFit : MonoBehaviour
{
    [SerializeField] private bool _saveProportions;

    private SpriteRenderer _render;
    private Vector3 _catcherSize;

    void LateUpdate()
    {
        _render = GetComponent<SpriteRenderer>();

        if (_render == null)
            return;

        var width = _render.sprite.bounds.size.x;
        var height = _render.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var w = worldScreenWidth / width;
        var h = worldScreenHeight / height;

        if (_catcherSize.x == w && _catcherSize.y == h)
            return;

        _catcherSize = new Vector3(w, h);

        transform.localScale = _saveProportions ? new Vector3(w < h ? h : w, h < w ? w : h) : new Vector3(w, h);
        transform.transform.position = new Vector3(Camera.main.transform.position.x, Camera.main.transform.position.y, transform.position.z);

    }
}