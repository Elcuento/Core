using UnityEngine;

namespace JTI.Examples
{
    public class PositionAndRotationHelper : MonoBehaviour
    {
        public static Vector3 GetMousePositionTo3D(Camera camera, Vector2 position, float zPos)
        {
            Vector3 mousePosition = position;
            mousePosition.z = zPos;

            var pos = camera.ScreenToWorldPoint(mousePosition);

            return pos;
        }
        public static Vector3 GetMouseWorldPositionInObjectInZPlane(Camera camera, Vector3 objectIn3DSpace)
        {
            var mousePosition = Input.mousePosition;
            mousePosition.z = camera.WorldToScreenPoint(objectIn3DSpace).z;

            var pos = camera.ScreenToWorldPoint(mousePosition);

            return pos;
        }

        public static Vector3 ScreenToUISpace(Canvas parentCanvas, Vector3 screenPos)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera,
                out var movePos);

            return parentCanvas.transform.TransformPoint(movePos);
        }

        public static Vector3 WorldToUISpace(Canvas parentCanvas, Vector3 worldPos)
        {
            var screenPos = Camera.main.WorldToScreenPoint(worldPos);

            RectTransformUtility.ScreenPointToLocalPointInRectangle(parentCanvas.transform as RectTransform, screenPos, parentCanvas.worldCamera,
                out var movePos);

            return parentCanvas.transform.TransformPoint(movePos);
        }

	}
}