using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Resources.Scripts
{
    public class PaperDragManager : DocumentBaseController
    {
        public bool isOnPaperContainer = false;
        
        private bool _lockedOnPaper = false;

        public Vector3 typewriterScale = Vector3.one;
        public Vector3 corkScale = Vector3.one;

        public float distanceMaxPaperContainer = 250;

        public float distancePaperContainer = 0;

        public string nameText;
        public string motvText;

        public Image[] imagesBackground;

        public PushpinController pushpin;

        public GameObject stampsParent;

        public TextMeshProUGUI inputTextPaper;
        public TextMeshProUGUI textDefault;

        public GameObject buttonSeparatePapers;

        public int countLines = 0;
        public int countLetters = 0;

        public bool isDefaultWritten = false;
        public bool canWrite = true;
        public bool paperSeparated = false;
        
        public StampController.TypeStamp typeStamp = StampController.TypeStamp.NONE;

        protected override void Start()
        {
            base.Start();
            
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

            if (TypewriterManager.instance.paperDragManager == null && canWrite && !paperSeparated)
            {
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
            }
            
            bool canShowButtonSeparate = isDefaultWritten && typeStamp != StampController.TypeStamp.NONE &&
                                         canWrite && !paperSeparated && !isOnPaperContainer;
            
            buttonSeparatePapers.transform.localScale = canShowButtonSeparate ? Vector3.one : Vector3.zero;
        }

        private void LateUpdate()
        {
            nameText = GameManager.instance.currentName;
            motvText = GameManager.instance.currentMotv;
            
            textDefault.text = nameText + "\n" + motvText;
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            if (isOnPaperContainer && !isDefaultWritten)
            {
                canDrag = false;
                return;
            }
            
            base.OnBeginDrag(eventData);
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
                Vector2 localPoint;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rt.parent as RectTransform, Input.mousePosition,
                    null, out localPoint);
                localPoint.x = 0;
                if (localPoint.y >= 0) rt.anchoredPosition = localPoint;
            }
        }

        protected override void EndDrag(PointerEventData eventData)
        {
            
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
            
            TypewriterManager.instance.SetPaper();
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