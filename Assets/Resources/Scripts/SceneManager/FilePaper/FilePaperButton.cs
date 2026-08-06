using System.Collections.Generic;
using DG.Tweening;
using Resources.Scripts;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FilePaperButton : MonoBehaviour
{
    public EventTrigger trigger;
    public TextMeshProUGUI textLetter;

    public RectTransform panelFileLetter;

    public GameObject prefabDocumentFile;
    public GameObject prefabPhotoFile;

    public GameObject container;
    
    public List<GameObject> documentFiles = new List<GameObject>();
    
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
        panelFileLetter.DOAnchorPosX(panelOpened ? -80 : 0, 0.3f);
    }
    
    private bool IsThisFilePaperOpened()
    {
        var filePaperButtonOpened = FilePaperManager.instance.GetFilePaperOpened();
        return filePaperButtonOpened == null || filePaperButtonOpened == this;
    }

    public GameObject InstancePaper(PaperDragManager paperDragManager)
    {
        var documentInstance = Instantiate(prefabDocumentFile, container.transform);
        documentFiles.Add(documentInstance);

        var documentFile = documentInstance.GetComponent<PaperFile>();
        documentFile.objDocument = paperDragManager;
        
        documentFile.SetData(paperDragManager);

        return documentInstance;
    }
    
    public GameObject InstancePhoto(PaperDragManager paperDragManager)
    {
        return null;
    }
}