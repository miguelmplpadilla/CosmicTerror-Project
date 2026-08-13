using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(LayoutElement))]
public class FileDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    public DragBaseManager objDocument;
    
    private LayoutElement _layoutElement;

    public RectTransform panelRt;

    private void Awake()
    {
        _layoutElement = GetComponent<LayoutElement>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        objDocument.gameObject.SetActive(true);
        
        _layoutElement.ignoreLayout = true;
        transform.localScale = Vector3.zero;
        
        objDocument.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        objDocument.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        objDocument.OnEndDrag(eventData);
        Destroy(gameObject);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        panelRt.DOKill();
        panelRt.DOAnchorPosY(4, 0.3f);
        ShowInfo(true);
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        panelRt.DOKill();
        panelRt.DOAnchorPosY(0, 0.3f);
        ShowInfo(false);
    }
    
    protected virtual void ShowInfo(bool show)
    {
        Debug.Log("ShowInfo");
    }

    public virtual void SetData(DragBaseManager dragBaseManager)
    {
        objDocument = dragBaseManager;
    }
}