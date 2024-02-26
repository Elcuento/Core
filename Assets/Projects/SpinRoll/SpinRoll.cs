using DG.Tweening;
using UnityEngine;

public class SpinRoll : MonoBehaviour
{
    public RectTransform _containerRollsPivot;
    public RectTransform _containerRolls;

    private void ResetPositions()
    {
        _containerRollsPivot.anchoredPosition = new Vector2();
        _containerRolls.anchoredPosition = new Vector2();
    }
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.S))
        {
            ResetPositions();

            var speed = 3;
            var slotSize = _containerRolls.sizeDelta.x / _containerRolls.childCount;
            var freeSpins = 2;

            var offset = _containerRolls.childCount % 2 == 0 ? 0 : slotSize / 2f;
            var movePlace = freeSpins * _containerRolls.childCount * slotSize + (int)(_containerRolls.childCount/2f) * slotSize + offset;

            var wait = true;

            var looseWidth = 0f;

            Debug.Log(movePlace);
            _containerRollsPivot.DOAnchorPosX(movePlace, speed).OnComplete(() =>
            {
                wait = false;
            }).SetEase(Ease.InOutSine).OnUpdate(() => RefreshRollPosition(slotSize, ref looseWidth));
        }
    }

    private void RefreshRollPosition(float slotSize, ref float looseWidth)
    {
        if (_containerRolls.transform.childCount == 0) return;

        Debug.Log(slotSize + looseWidth);
        if (_containerRollsPivot.anchoredPosition.x > slotSize + looseWidth)
        {
            looseWidth += slotSize;
            var last = _containerRolls.transform.GetChild(_containerRolls.childCount - 1);
            last.SetAsFirstSibling();
            _containerRolls.anchoredPosition -= new Vector2(slotSize, 0);
        }
    }
}
