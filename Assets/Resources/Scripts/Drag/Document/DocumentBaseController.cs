using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentBaseController : DragBaseManager
{
    public DocumentData documentData;
    
    private InterBaseController interContact;
    private InterBaseController lastInterContact;

    protected override void Awake()
    {
        base.Awake();

        if (documentData != null) documentData.documentBaseController = this;
    }

    protected override void Drag(PointerEventData eventData)
    {
        base.Drag(eventData);

        if (DialogueController.instance.isPlayingDialogue) return;
        
        interContact = GetInterContact();

        Debug.Log("Inter Contact: "+interContact);

        if (interContact != null && !interContact.CanInteractWith(this)) interContact = null;
        
        if (interContact != null) interContact.ShowIcon(true);
        if (interContact == null && lastInterContact != null) lastInterContact.ShowIcon(false);
        
        lastInterContact = interContact;
    }

    protected override void EndDrag(PointerEventData eventData)
    {
        base.EndDrag(eventData);
        
        if (DialogueController.instance.isPlayingDialogue) return;
        
        if (interContact != null)
        {
            interContact.Inter(documentData);
            interContact.ShowIcon(false);
            interContact = null;
        }
        
        if (lastInterContact != null) lastInterContact.ShowIcon(false);

        lastInterContact = null;
        
        MouseController.instance.ShowAskIcon(false);
    }
}
