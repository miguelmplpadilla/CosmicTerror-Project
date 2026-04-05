using System.Collections.Generic;
using UnityEngine;

namespace Resources.Scripts
{
    [RequireComponent(typeof(UILineRenderer))]
    public class LineController : MonoBehaviour
    {
        public RectTransform pointA;
        public RectTransform pointB;

        private UILineRenderer _lineRenderer;

        private void Awake()
        {
            _lineRenderer = GetComponent<UILineRenderer>();
        }

        private void Update()
        {
            DrawLine();
        }

        private void DrawLine()
        {
            if (pointA == null || pointB == null) return;

            List<Vector2> positions = new List<Vector2>();
            positions.Add(CanvasToAnchoredPosition(pointA));
            positions.Add(CanvasToAnchoredPosition(pointB));
            
            _lineRenderer.ModifyLinePoints(positions.ToArray());
        }
        
        public void SetPoints(RectTransform a, RectTransform b)
        {
            pointA = a;
            pointB = b;
        }
        
        private Vector2 CanvasToAnchoredPosition(RectTransform rectTransform)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, rectTransform.position);
            RectTransform canvasRect = _lineRenderer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);
            return localPoint;
        }
    }
}