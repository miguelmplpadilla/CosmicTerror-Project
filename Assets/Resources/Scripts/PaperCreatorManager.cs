using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class PaperCreatorManager : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public GameObject prefabPaper;
    public GameObject parentContainer;

    private PaperDragManager paperDragManager;
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        GameObject paperInstantiated = Instantiate(prefabPaper, parentContainer.transform);
        paperDragManager = paperInstantiated.GetComponent<PaperDragManager>();
        paperDragManager.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        paperDragManager.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        paperDragManager.OnEndDrag(eventData);
        paperDragManager = null;
    }
}
