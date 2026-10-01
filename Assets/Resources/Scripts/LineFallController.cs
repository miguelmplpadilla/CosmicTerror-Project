using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Resources.Scripts
{
    [RequireComponent(typeof(UILineRenderer))]
    public class LineFallController : MonoBehaviour
    {
        public RectTransform pointA;
        public RectTransform pointB;

        public bool connecting = false;

        public int cantPoints = 20;
        public float maxSag = 50f;
        public float curve = 2f;
        
        public float maxDistance = 10f;

        public UILineRenderer lineRenderer;

        private void Awake()
        {
            lineRenderer = GetComponent<UILineRenderer>();
            lineRenderer.raycastTarget = true;

            GraphicRaycaster graphicRaycaster = GetComponent<GraphicRaycaster>();
            if (graphicRaycaster != null)
                graphicRaycaster.ignoreReversedGraphics = false;
        }

        private void Update()
        {
            DrawLines();
        }

        private void DrawLines()
        {
            if (pointA == null || pointB == null) return;
            
            float distance = Vector2.Distance(pointA.position, pointB.position);

            float finalSag = maxDistance <= 0f
                ? maxSag
                : (1f - Mathf.Clamp01(distance / maxDistance)) * maxSag;
            
            List<Vector2> fakePoints = GetCatenaryPoints(pointA.position, pointB.position, cantPoints, finalSag, curve);
            List<Vector2> linePoints = WorldPointsToAnchoredPoints(fakePoints);

            lineRenderer.ModifyLinePoints(linePoints.ToArray());
        }
        
        private Vector2 CanvasToAnchoredPosition(Vector2 positionRt)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(Camera.main, positionRt);
            RectTransform canvasRect = lineRenderer.GetComponentInParent<Canvas>().GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, Camera.main, out Vector2 localPoint);
            return localPoint;
        }

        private List<Vector2> WorldPointsToAnchoredPoints(List<Vector2> worldPoints)
        {
            List<Vector2> anchoredPoints = new List<Vector2>();

            for (int i = 0; i < worldPoints.Count; i++)
                anchoredPoints.Add(CanvasToAnchoredPosition(worldPoints[i]));

            return anchoredPoints;
        }
        
        private List<Vector2> GetCatenaryPoints(Vector2 pointA, Vector2 pointB, int pointCount, float sag, float curve = 2f)
        {
            List<Vector2> points = new List<Vector2>();

            if (pointCount < 12)
                pointCount = 12;

            curve = Mathf.Max(0.01f, curve);
            float denominator = Cosh(curve) - 1f;

            for (int i = 0; i < pointCount; i++)
            {
                float t = i / (float)(pointCount - 1);

                Vector2 point = Vector2.Lerp(pointA, pointB, t);

                float x = Mathf.Lerp(-curve, curve, t);
                float catenaryShape = 1f - ((Cosh(x) - 1f) / denominator);

                point.y -= catenaryShape * sag;

                points.Add(point);
            }

            return points;
        }

        private float Cosh(float value)
        {
            return (float)System.Math.Cosh(value);
        }

        public void SetPoints(RectTransform a, RectTransform b)
        {
            pointA = a;
            pointB = b;
        }

        public void RemoveLine()
        {
            if (connecting) return;
            ShowIcon(false);
            Destroy(gameObject);
        }
        
        public void ShowIcon(bool show)
        {
            if (connecting) return;
            MouseController.instance.ShowRemoveIcon(show);
        }
    }
}
