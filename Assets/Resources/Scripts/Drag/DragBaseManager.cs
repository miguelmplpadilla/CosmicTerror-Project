using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
[RequireComponent(typeof(Canvas))]
[RequireComponent(typeof(GraphicRaycaster))]
public class DragBaseManager : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    protected RectTransform rt;
    protected Canvas canvas;

    protected bool canDrag = true;
    protected bool isDraging = true;

    protected bool isAnimating = false;

    public Vector3 scaleBig;
    public Vector3 scaleSmall;

    public Vector3 positionShadow = new Vector3(20, -20, 0);

    public GameObject objBig;
    public GameObject objSmall;
    public GameObject currentObjectSize;

    protected GameObject deskContainer;

    protected virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
        canvas = GetComponent<Canvas>();
    }

    protected virtual void Start()
    {
        deskContainer = GameObject.Find("DeskContiner");
        
        canvas.overrideSorting = false;
        canvas.sortingOrder = 1;
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (isAnimating) return;
        transform.SetParent(GameManager.instance.panelGlobalObjects.transform);
        transform.SetAsLastSibling();
        canDrag = true;
        isDraging = true;
        canvas.overrideSorting = true;
        canvas.sortingOrder = 4;
        ShowShadow(true);
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
        GameManager.instance.currentDraggingObject = this;
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
        
        canvas.overrideSorting = false;
        canvas.sortingOrder = 1;
        
        ShowShadow(false);
        EndDrag(eventData);
        GameManager.instance.currentDraggingObject = null;
    }

    protected virtual void EndDrag(PointerEventData eventData)
    {
        if (IsOnBackContainer())
        {
            isAnimating = true;
            transform.DOMoveY(deskContainer.transform.position.y, 0.3f).OnComplete(() =>
            {
                isAnimating = false;
            }).OnUpdate(() => SetSize(GetContainerType()));
        }
    }

    private RaycastHit2D[] GetContactObject()
    {
        return Physics2D.RaycastAll(transform.position, Vector2.zero, Mathf.Infinity);
    }

    protected InterBaseController GetInterContact()
    {
        foreach (var obj in GetContactObject())
        {
            if (obj.collider.TryGetComponent(out InterBaseController interObj))
                return interObj;
        }

        return null;
    }

    protected void SetSize(bool size)
    {
        transform.localScale = size ? scaleBig : scaleSmall;
        
        objBig.SetActive(false);
        objSmall.SetActive(false);
        
        currentObjectSize = size ? objBig : objSmall;
        currentObjectSize.SetActive(true);
    }

    protected bool GetContainerType()
    {
        RaycastHit2D[] hits = GetContactObject();

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Big")) return true;
            if (hit.collider.CompareTag("Small")) return false;
        }

        return false;
    }

    protected GameObject GetContainer()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(Input.mousePosition, Vector2.zero, Mathf.Infinity);

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Big") || hit.collider.CompareTag("Small")) 
                return hit.collider.gameObject;
        }

        return null;
    }

    protected bool IsOnBackContainer()
    {
        GameObject container = GetContainer();
        
        return container != null && container.name.Equals("BackContiner");
    }

    private void ShowShadow(bool show)
    {
        objBig.transform.GetChild(0).DOLocalMove(show ? positionShadow : Vector3.zero, 0.2f);
        objSmall.transform.GetChild(0).DOLocalMove(show ? positionShadow : Vector3.zero, 0.2f);
    }
}
