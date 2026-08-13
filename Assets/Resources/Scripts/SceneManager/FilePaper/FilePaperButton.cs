using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class FilePaperButton : MonoBehaviour
{
    public EventTrigger trigger;
    public TextMeshProUGUI textLetter;

    public RectTransform fileButtonRt;
    public RectTransform panelFileLetter;
    public RectTransform panelFilePaperContainer;
    
    public GameObject parent;

    public bool panelOpened = false;

    public Canvas canvas;
    
    private void Awake()
    {
        AddEvent(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
        AddEvent(trigger, EventTriggerType.PointerExit, OnPointerExit);
        AddEvent(trigger, EventTriggerType.PointerUp, OnPointerUp);
        AddEvent(trigger, EventTriggerType.PointerDown, OnPointerDown);
    }

    private void AddEvent(
        EventTrigger trigger,
        EventTriggerType type,
        UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = type
        };

        entry.callback.AddListener(callback);
        trigger.triggers.Add(entry);
    }

    private void OnPointerEnter(BaseEventData data)
    {
        if (!IsThisFilePaperOpened()) return;
        
        if (!panelOpened) canvas.sortingOrder = 2;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosX(panelOpened ? -75 : -5, 0.3f);
    }

    private void OnPointerExit(BaseEventData data)
    {
        if (!IsThisFilePaperOpened()) return;
        
        if (!panelOpened) canvas.sortingOrder = 1;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosX(panelOpened ? -80 : 0, 0.3f);
    }
    
    private void OnPointerDown(BaseEventData data)
    {
        Debug.Log("OnPointerDown");
    }

    private void OnPointerUp(BaseEventData data)
    {
        if (!IsThisFilePaperOpened()) return;
        
        panelOpened = !panelOpened;
        
        parent.transform.DOScale(panelOpened ? 2f : 1, 0.3f);
        
        canvas.sortingOrder = panelOpened ? 3 : 1;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosY(panelOpened ? panelFilePaperContainer.sizeDelta.y + 13 : 0, 0.3f);
        panelFileLetter.DOAnchorPosX(panelOpened ? -80 : 0, 0.3f);
    }
    
    private bool IsThisFilePaperOpened()
    {
        var filePaperButtonOpened = FilePaperManager.instance.GetFilePaperOpened();
        return filePaperButtonOpened == null || filePaperButtonOpened == this;
    }
}