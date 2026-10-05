using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentBaseController : DragBaseManager
{
    public DocumentData documentData;
    
    private InterBaseController interContact;
    private InterBaseController lastInterContact;
    
    public PushpinController pushpin;
    
    public Vector3 corkScale = Vector3.one;

    protected override void Awake()
    {
        base.Awake();

        if (documentData != null) documentData.documentBaseController = this;
    }

    protected override void Start()
    {
        base.Start();

        pushpin.documentBaseController = this;
        
        EventBus<CheckInsideCorkboard>.Register(new EventBinding<CheckInsideCorkboard>(IsInCorkBoard, gameObject));
    }
    
    private void OnDestroy()
    {
        EventBus<CheckInsideCorkboard>.Deregister(new EventBinding<CheckInsideCorkboard>(IsInCorkBoard, gameObject));
    }

    protected override void Drag(PointerEventData eventData)
    {
        base.Drag(eventData);
        
        GameObject currentContainer = GetContainer();
        if (currentContainer != null && currentContainer.name.Equals("Cork"))
        {
            pushpin.ChangeAlpha(1);
            pushpin.gameObject.SetActive(true);
            transform.localScale = corkScale;
        }
        else
        {
            pushpin.ChangeAlpha(0.5f);
        }
        
        interContact = GetInterContact();

        if (interContact != null && !interContact.CanInteractWith(this)) interContact = null;
        
        if (interContact != null) interContact.ShowIcon(true);
        if (interContact == null && lastInterContact != null) lastInterContact.ShowIcon(false);
        
        lastInterContact = interContact;
    }

    protected override void EndDrag(PointerEventData eventData)
    {
        base.EndDrag(eventData);
        
        pushpin.ChangeAlpha(1);
        
        if (interContact != null)
        {
            interContact.Inter(this);
            interContact.ShowIcon(false);
            interContact = null;
        }
        
        if (lastInterContact != null) lastInterContact.ShowIcon(false);

        lastInterContact = null;
        
        MouseController.instance.ShowAskIcon(false);
        
        GameObject currentContainer = GetContainer();
        if (currentContainer != null && currentContainer.name.Equals("Cork"))
        {
            transform.SetParent(currentContainer.transform);
        }
        else
        {
            pushpin.gameObject.SetActive(false);
            pushpin.RemoveAllLines();
        }
    }
    
    private void IsInCorkBoard()
    {
        if (!transform.parent.name.Equals("Cork"))
            pushpin.RemoveAllLines();
    }
}
