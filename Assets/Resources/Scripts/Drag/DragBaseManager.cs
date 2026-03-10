using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DragBaseManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected RectTransform rt;

    protected bool canDrag = true;
    protected bool isDraging = true;

    protected bool isAnimating = false;

    public Vector3 scaleBig;
    public Vector3 scaleSmall;

    public GameObject objBig;
    public GameObject objSmall;

    private GameObject deskContainer;

    protected virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    protected virtual void Start()
    {
        deskContainer = GameObject.Find("DeskContiner");
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isAnimating) return;
        transform.SetParent(GameManager.instance.panelGlobalObjects.transform);
        canDrag = true;
        isDraging = true;
        BeginDrag(eventData);
    }
    
    protected virtual void BeginDrag(PointerEventData eventData)
    {
        rt.position = Input.mousePosition;
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        SetSize(GetContainerType());
        Drag(eventData);
    }

    protected virtual void Drag(PointerEventData eventData)
    {
        rt.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!canDrag) return;
        isDraging = false;
        
        EndDrag(eventData);
    }

    protected virtual void EndDrag(PointerEventData eventData)
    {
        GameObject container = GetContainer();
        if (container != null)
        {
            if (container.name.Equals("BackContiner"))
            {
                isAnimating = true;
                transform.DOMoveY(deskContainer.transform.position.y, 0.3f).OnComplete(() =>
                {
                    isAnimating = false;
                }).OnUpdate(() => SetSize(GetContainerType()));
            }
        }
    }

    protected void SetSize(bool size)
    {
        transform.localScale = size ? scaleBig : scaleSmall;
        
        objBig.SetActive(false);
        objSmall.SetActive(false);
        
        if (size) objBig.SetActive(true);
        else objSmall.SetActive(true);
    }

    protected bool GetContainerType()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.zero, Mathf.Infinity);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Big")) return true;
            if (hit.collider.CompareTag("Small")) return false;
        }

        return false;
    }

    private GameObject GetContainer()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.zero, Mathf.Infinity);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Big") || hit.collider.CompareTag("Small")) 
                return hit.collider.gameObject;
        }

        return null;
    }
}
