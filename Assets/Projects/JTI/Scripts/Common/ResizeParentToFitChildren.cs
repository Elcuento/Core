using UnityEngine;
using UnityEngine.UI;

namespace JTI.Examples
{
    [ExecuteInEditMode]
    public class ResizeParentToFitChildren : MonoBehaviour
    {
        public float extraOffset;

        public float maxWidth = 200;
        public float maxHeight = 200;

        [SerializeField] private bool _execute;

        public Transform Parent;

        private void Update()
        {
            if (!_execute)
                return;

            _execute = false;
            ResizeParent();
        }


        void ResizeParent()
        {
            if (Parent == null)
                Parent = transform.parent;

            if (Parent == null)
            {
                Debug.Log("No parent");
                return;
            }

            transform.localScale = new Vector3(1, 1, 1);

            float maxX = float.MinValue;
            float maxY = float.MinValue;

            foreach (var childRectTransform in transform.GetComponentsInChildren<RectTransform>())
            {
                if (childRectTransform != null)
                {
                    var pos = Parent.InverseTransformPoint(new Vector3(childRectTransform.position.x,
                        childRectTransform.position.y));

                    var pivotOffset = new Vector2(
                        childRectTransform.rect.width * (0.5f - childRectTransform.pivot.x) *
                        childRectTransform.localScale.x,
                        childRectTransform.rect.height * (0.5f - childRectTransform.pivot.y) *
                        childRectTransform.localScale.y);

                    var childSize = new Vector2(childRectTransform.rect.width * childRectTransform.localScale.x,
                        childRectTransform.rect.height * childRectTransform.localScale.y);

                    var childPosition = new Vector2(pos.x * childRectTransform.localScale.x,
                        pos.y * childRectTransform.localScale.y);

                    Debug.Log(pivotOffset);
                    float childRightX = Mathf.Abs(childPosition.x) - pivotOffset.x + childSize.x / 2;
                    float childTopY = Mathf.Abs(childPosition.y) - pivotOffset.y + childSize.y / 2;

                    Debug.Log(childRectTransform + ":" + childRightX + ":" + childSize);

                    maxX = Mathf.Max(maxX, childRightX);
                    maxY = Mathf.Max(maxY, childTopY);
                }
            }

            maxX *= transform.localScale.x;
            maxY *= transform.localScale.y;

            var maxValues = new Vector2(maxX, maxY);

            var rel = new Vector2(maxWidth / (maxValues.x * 2f), maxHeight / (maxValues.y * 2f));

            var targetScale = rel.x < rel.y ? rel.x : rel.y;

            transform.localScale = new Vector3(targetScale + extraOffset, targetScale + extraOffset,
                targetScale + extraOffset);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;

            Gizmos.DrawWireCube(transform.position, new Vector3(maxWidth, maxHeight));
        }

    }
}