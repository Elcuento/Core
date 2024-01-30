using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spinning : MonoBehaviour
{
    public RectTransform _containerRolls;
    public RectTransform _containerRollsPivot;
    public RectTransform _containerRollsParent;
    // Only multiply by 2 !!

    private void Start()
    {
        StartCoroutine(RollCoroutine());
    }
    private void RefreshRollPosition(float sizeSlot, ref float looseWidth)
    {
        if (_containerRolls.transform.childCount == 0) return;

        if (_containerRollsParent.anchoredPosition.x > sizeSlot + looseWidth)
        {
            looseWidth += sizeSlot;
            var last = _containerRolls.transform.GetChild(_containerRolls.childCount - 1);
            last.SetAsFirstSibling();
            _containerRolls.anchoredPosition -= new Vector2(sizeSlot, 0);
        }


    }
    private IEnumerator RollCoroutine()
    {
        yield return null;

        var speed = 3;
        var freeSpins = 7;
        var slotSize = _containerRolls.sizeDelta.x / _containerRolls.childCount;
        var moveTo = (_containerRolls.childCount-1) + Random.Range(0, _containerRolls.childCount-1);

        var movePlace = moveTo * freeSpins * slotSize;

        var wait = true;

        var looseWidth = 0f;

        _containerRollsParent.DOAnchorPosX(movePlace, speed).OnComplete(() =>
        {
            wait = false;
        }).SetEase(Ease.InOutSine).OnUpdate(()=>RefreshRollPosition(slotSize, ref looseWidth));

        yield return new WaitUntil(() => !wait);

        var winner = _containerRolls.transform.GetChild(_containerRolls.transform.childCount / 2);

    }

}
