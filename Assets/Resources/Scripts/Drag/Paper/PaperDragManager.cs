using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Resources.Scripts
{
    public class PaperDragManager : DragBaseManager
    {
        public bool isOnPaperContainer = false;
        public bool isOnFileDesk = false;
        
        private bool _lockedOnPaper = false;

        public Vector3 typewriterScale = Vector3.one;
        public Vector3 corkScale = Vector3.one;

        public float distanceMaxPaperContainer = 250;
        public float distanceMaxFileDesk = 300;

        public float distancePaperContainer = 0;
        public float distanceFileDesk = 0;

        private GameObject _fileDesk;
        public PushpinController pushpin;

        public TextMeshProUGUI inputTextPaper;

        public int countLines = 0;
        public int countLetters = 0;

        protected override void Start()
        {
            base.Start();
            
            _fileDesk = GameObject.Find("FilePaper");
            
            EventBus<CheckInsideCorkboard>.Register(new EventBinding<CheckInsideCorkboard>(IsInCorkBoard, gameObject));
        }

        private void OnDestroy()
        {
            EventBus<CheckInsideCorkboard>.Deregister(new EventBinding<CheckInsideCorkboard>(IsInCorkBoard, gameObject));
        }

        private void Update()
        {
            distancePaperContainer = Vector2.Distance(
                TypewriterManager.instance.centerPaper.transform.position,
                transform.position
            );

            if (!_lockedOnPaper)
            {
                isOnPaperContainer = distancePaperContainer < distanceMaxPaperContainer;

                if (isOnPaperContainer)
                    _lockedOnPaper = true;
            }
            else
            {
                isOnPaperContainer = distancePaperContainer < (distanceMaxPaperContainer + 30);

                if (!isOnPaperContainer)
                    _lockedOnPaper = false;
            }
            
            distanceFileDesk = Mathf.Abs(transform.position.x - _fileDesk.transform.position.x);

            isOnFileDesk = !_lockedOnPaper && distanceFileDesk < distanceMaxFileDesk;
        }

        protected override void BeginDrag(PointerEventData eventData)
        {
            base.BeginDrag(eventData);

            _lockedOnPaper = isOnPaperContainer;

            if (isOnPaperContainer)
            {
                TypewriterManager.instance.paperDragManager = null;
                TypewriterManager.instance.ExtendMaskPaper(true);
                return;
            }
            
            transform.SetParent(GameManager.instance.panelGlobalObjects.transform);
        }

        protected override void Drag(PointerEventData eventData)
        {
            Vector3 finalRotation = Vector3.zero;

            if (isOnFileDesk && !isOnPaperContainer)
            {
                finalRotation = new Vector3(0, 0, 90);
            }

            transform.DORotate(finalRotation, 0.3f);
            
            pushpin.gameObject.SetActive(false);
            
            GameObject currentContainer = GetContainer();
            if (currentContainer != null && currentContainer.name.Equals("Cork"))
            {
                transform.localScale = corkScale;
                pushpin.gameObject.SetActive(!isOnPaperContainer);
            }

            if (isOnPaperContainer && IsTheSameInputPaper())
            {
                SetSize(true);
                transform.localScale = typewriterScale;
            }
            else
            {
                base.Drag(eventData);
            }
            
            transform.SetParent(isOnPaperContainer && IsTheSameInputPaper()
                ? TypewriterManager.instance.paperRT.transform
                : GameManager.instance.panelGlobalObjects.transform);
            
            if (isOnPaperContainer && IsTheSameInputPaper())
            {
                pushpin.RemoveAllLines();
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt.parent as RectTransform, Input.mousePosition,
                    null, out localPoint);
                localPoint.x = 0;
                if (localPoint.y >= 0) rt.anchoredPosition = localPoint;
            }
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            Vector3 finalRotation = Vector3.zero;

            if (isOnFileDesk) finalRotation = new Vector3(0, 0, 90);

            transform.DOKill();
            transform.DORotate(finalRotation, 0.3f);
            
            if (isOnPaperContainer && IsTheSameInputPaper())
            {
                TypewriterManager.instance.paperDragManager = this;
                
                TypewriterManager.instance.CalculateVerticalPosition();
                TypewriterManager.instance.CalculateHorizontalPosition();
                
                PutPaperInTypewriter();
                return;
            }
            
            GameObject currentContainer = GetContainer();
            if (currentContainer != null && currentContainer.name.Equals("Cork"))
            {
                transform.SetParent(currentContainer.transform);
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
            
            if (IsOnBackContainer())
            {
                isAnimating = true;
                transform.DOMoveY(deskContainer.transform.position.y, 0.3f).OnComplete(() =>
                {
                    isAnimating = false;
                }).OnUpdate(() =>
                {
                    float distancePaperContainer = Vector2.Distance(TypewriterManager.instance.centerPaper.transform.position,
                        transform.position);
                    if (distancePaperContainer < distanceMaxPaperContainer)
                    {
                        rt.DOKill();
                        transform.SetParent(TypewriterManager.instance.paperRT.transform);
                        SetSize(true);
                        transform.localScale = typewriterScale;
                        transform.DORotate(Vector3.zero,0);
                        rt.anchoredPosition = new Vector2(0, rt.anchoredPosition.y);
                        PutPaperInTypewriter();
                        return;
                    }
                    
                    SetSize(GetContainerType());
                });
            }
        }

        private void PutPaperInTypewriter()
        {
            pushpin.RemoveAllLines();
            isAnimating = true;
            TypewriterManager.instance.ExtendMaskPaper(false);
            rt.DOAnchorPosY(0, 0.3f).OnComplete(() =>
            {
                isAnimating = false;
            });
        }

        private bool IsTheSameInputPaper()
        {
            return TypewriterManager.instance.paperDragManager == null ||
                   TypewriterManager.instance.paperDragManager == this;
        }

        private void IsInCorkBoard()
        {
            if (!transform.parent.name.Equals("Cork"))
                pushpin.RemoveAllLines();
        }
    }
}