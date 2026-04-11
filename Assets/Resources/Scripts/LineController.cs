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
        private UILineRenderer _lineRendererBold1;
        private UILineRenderer _lineRendererBold2;

        private void Awake()
        {
            _lineRenderer = GetComponent<UILineRenderer>();
            _lineRendererBold1 = transform.GetChild(0).GetComponent<UILineRenderer>();
            _lineRendererBold2 = transform.GetChild(1).GetComponent<UILineRenderer>();
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

            _lineRendererBold1.transform.localScale = Vector3.one;
            _lineRendererBold2.transform.localScale = Vector3.one;

            Vector2 positionDirectionRightDeskA = GetPositionOnLine(pointA.position, 
                pointB.position, "RightDesk");
            Vector2 positionDirectionDeskA = GetPositionOnLine(pointA.position, 
                pointB.position, "Desk");
            Vector2 positionDirectionRightDeskB = GetPositionOnLine(pointB.position, 
                pointA.position, "RightDesk");
            Vector2 positionDirectionDeskB = GetPositionOnLine(pointB.position, 
                pointA.position, "Desk");

            Vector2 finalPositionA = Vector2.Distance(positionDirectionRightDeskA, pointA.position) < 1 ||
                                     positionDirectionRightDeskA == Vector2.zero
                ? positionDirectionDeskA
                : positionDirectionRightDeskA;

            Vector2 finalPositionB = Vector2.Distance(positionDirectionRightDeskB, pointB.position) < 1 ||
                                     positionDirectionRightDeskB == Vector2.zero
                ? positionDirectionDeskB
                : positionDirectionRightDeskB;
            
            float distanceA = Vector2.Distance(finalPositionA, pointA.position);
            float distanceB = Vector2.Distance(finalPositionB, pointB.position);

            Debug.Log("Distance A: "+distanceA+ " Distance B: "+distanceB);

            if (finalPositionA != Vector2.zero && distanceA > 1)
            {
                List<Vector2> positionsLine = new List<Vector2>();
                positionsLine.Add(CanvasToAnchoredPosition(finalPositionA));
                positionsLine.Add(CanvasToAnchoredPosition(pointB.position));
            
                _lineRendererBold1.ModifyLinePoints(positionsLine.ToArray());
            }
            else
            {
                _lineRendererBold1.transform.localScale = Vector3.zero;
            }
            
            if (finalPositionB != Vector2.zero && distanceB > 1)
            {
                List<Vector2> positionsLine = new List<Vector2>();
                positionsLine.Add(CanvasToAnchoredPosition(finalPositionB));
                positionsLine.Add(CanvasToAnchoredPosition(pointA.position));
            
                _lineRendererBold2.ModifyLinePoints(positionsLine.ToArray());
            }
            else
            {
                _lineRendererBold2.transform.localScale = Vector3.zero;
            }
        }
        
        private Vector2 GetPositionOnLine(Vector2 positionA, Vector2 positionB, string keyName)
        {
            Vector2 origin = positionA;
            Vector2 direction = (positionB - origin).normalized;
            float distance = Vector2.Distance(positionA, positionB);

            RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject.name.Equals(keyName))
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