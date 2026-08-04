using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class DocumentBaseController : DragBaseManager
{
    public DocumentData documentData;
    
    private NPCBase npcContact;

    protected override void Awake()
    {
        base.Awake();

        documentData.obj = gameObject;
    }

    protected override void Drag(PointerEventData eventData)
    {
        base.Drag(eventData);

        if (DialogueController.instance.isPlayingDialogue) return;
        
        npcContact = GetNPCContact();
        MouseController.instance.ShowAskIcon(npcContact != null);
    }

    protected override void EndDrag(PointerEventData eventData)
    {
        base.EndDrag(eventData);
        
        if (DialogueController.instance.isPlayingDialogue) return;
        
        if (npcContact != null)
        {
            npcContact.AskObject(documentData);
            npcContact = null;
        }
        
        MouseController.instance.ShowAskIcon(false);
    }
}
