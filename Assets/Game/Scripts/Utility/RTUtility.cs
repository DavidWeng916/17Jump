using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Live17Game
{
    public static class CoordinateUtility
    {
        public static Vector2 ConvertWorldPointToCanvas(CoordConvertData coordConvertData)
        {
            return ConvertWorldPointToCanvas(coordConvertData.Camera, coordConvertData.Canvas, coordConvertData.Container, coordConvertData.WorldPoint);
        }

        public static Vector2 ConvertWorldPointToCanvas(Camera camera, Canvas canvas, RectTransform container, Vector3 worldPoint)
        {
            Camera uiCamera = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, worldPoint);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(container, screenPoint, uiCamera, out Vector2 uiPosition);

            return uiPosition;
        }
    }
}