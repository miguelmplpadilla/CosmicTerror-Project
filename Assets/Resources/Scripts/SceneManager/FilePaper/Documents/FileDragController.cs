using UnityEngine;
using UnityEngine.EventSystems;

public class FileDragController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public DragBaseManager objDocument;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        objDocument.gameObject.SetActive(true);
        gameObject.SetActive(false);
        
        objDocument.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        objDocument.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        objDocument.OnEndDrag(eventData);
    }
    
    public virtual void SetData(DragBaseManager dragBaseManager)
    {
        
    }
}