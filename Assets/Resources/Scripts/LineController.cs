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
        private UILineRenderer _lineRendererBold;

        private void Awake()
        {
            _lineRenderer = GetComponent<UILineRenderer>();
            _lineRendererBold = transform.GetChild(0).GetComponent<UILineRenderer>();
        }

        private void Update()
        {
            DrawLines();
        }

        private void DrawLines()
        {
            if (pointA == null || pointB == null) return;

            List<Vector2> positions = new List<Vector2>();
            positions.Add(CanvasToAnchoredPosition(pointA.position));
            positions.Add(CanvasToAnchoredPosition(pointB.position));
            
            _lineRenderer.ModifyLinePoints(positions.ToArray());
            
            DrawLineBold();
        }

        private void DrawLineBold()
        {
            if (pointA == null || pointB == null) return;

            _lineRendererBold.transform.localScale = Vector3.one;

            Vector2 positionDirectionA = GetPositionOnLine(pointA.position, pointB.position);
            Vector2 positionDirectionB = GetPositionOnLine(pointB.position, pointA.position);
            
            //TODO: Hacer que se dibuje la linea desde el lado mas lejano
            
            _lineRendererBold.transform.localScale = Vector3.zero;
        }
        
        private Vector2 GetPositionOnLine(Vector2 positionA, Vector2 positionB)
        {
            Vector2 origin = positionA;
            Vector2 direction = (positionB - origin).normalized;
            float distance = Vector2.Distance(positionA, positionB);

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.name.Equals("RightDesk") || hit.collider.gameObject.name.Equals("Desk"))
                    return hit.point;
            }

            return Vector2.zero;
        }
        
        public void SetPoints(RectTransform a, RectTransform b)
        {
            pointA = a;
            pointB = b;
        }
        
        private Vector2 CanvasToAnchoredPosition(Vector2 positionRt)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, positionRt);
            RectTransform canvasRect = _lineRenderer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);
            return localPoint;
        }
    }
}