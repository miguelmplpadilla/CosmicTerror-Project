using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class PaperDragManager : DragBaseManager
    {
        public bool isOnPaperContainer = false;
        public bool isOnFileDesk = false;

        public Vector3 typewriterScale = Vector3.one;

        private GameObject _fileDesk;

        protected override void Start()
        {
            base.Start();
            
            _fileDesk = GameObject.Find("FilePaper");
        }

        protected override void Drag(PointerEventData eventData)
        {
            float distancePaperContainer = Vector2.Distance(TypewriterManager.instance.centerPaper.transform.position,
                transform.position);
            isOnPaperContainer = distancePaperContainer < 250;
            
            float distanceFileDesk = Vector2.Distance(new Vector2(_fileDesk.transform.position.x, 0),
                new Vector2(transform.position.x, 0));
            isOnFileDesk = distanceFileDesk < 300;
            
            if (!isOnPaperContainer) base.Drag(eventData);
            else
            {
                SetSize(true);
                transform.localScale = typewriterScale;
            }

            Vector3 finalRotation = Vector3.zero;

            if (isOnFileDesk)
            {
                finalRotation = new Vector3(0, 0, 90);
            }

            transform.DORotate(finalRotation, 0.5f);
            
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt.parent as RectTransform, Input.mousePosition,
                null, out localPoint);
            if (isOnPaperContainer) localPoint.x = 0;
            
            if (localPoint.y >= 0 || !isOnPaperContainer)
                rt.anchoredPosition = localPoint;

            transform.SetParent(isOnPaperContainer
                ? TypewriterManager.instance.paperRT.transform
                : GameManager.instance.panelGlobalObjects.transform);
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            if (isOnPaperContainer)
            {
                isAnimating = true;
                rt.DOAnchorPosY(0, 0.3f).OnComplete(() =>
                {
                    isAnimating = false;
                });
                return;
            }

            if (isOnFileDesk)
            {
                transform.SetParent(_fileDesk.transform.GetChild(0));
                isAnimating = true;
                rt.DOAnchorPosX(0, 0.3f).OnComplete(() =>
                {
                    isAnimating = false;
                }).OnUpdate(() => SetSize(GetContainerType()));
                return;
            }
            
            base.EndDrag(eventData);
        }
    }
}