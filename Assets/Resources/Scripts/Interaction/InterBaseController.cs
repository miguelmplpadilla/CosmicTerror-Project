using Resources.Scripts;
using UnityEngine;

public class InterBaseController : MonoBehaviour
{
    public virtual bool CanInteractWith(DragBaseManager objInter)
    {
        return objInter is DocumentBaseController;
    }

    public virtual void Inter(DocumentData documentData)
    {
        
    }

    public virtual void ShowIcon(bool show)
    {
        Debug.Log("ShowIcon");
        MouseController.instance.ShowAskIcon(show);
    }
}