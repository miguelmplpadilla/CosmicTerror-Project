﻿using System;
 using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FilePaperButton : MonoBehaviour
{
    public EventTrigger trigger;
    public Button buttonReturn;
    
    public TextMeshProUGUI textLetter;

    public RectTransform fileButtonRt;
    public RectTransform panelFileLetter;
    public RectTransform panelFilePaperContainer;

    private GameObject originalParent;
    public Vector3 originalLocalPosition;
    
    public GameObject panelAllFile;
    
    public GameObject parent;

    public bool panelOpened = false;
    public bool panelAnimating = false;

    public Canvas canvas;
    public Canvas allPanelCanvas;
    
    private void Awake()
    {
        AddEvent(trigger, EventTriggerType.PointerEnter, OnPointerEnter);
        AddEvent(trigger, EventTriggerType.PointerExit, OnPointerExit);
        AddEvent(trigger, EventTriggerType.PointerUp, OnPointerUp);
        AddEvent(trigger, EventTriggerType.PointerDown, OnPointerDown);
        
        buttonReturn.onClick.AddListener(() => StartCoroutine(ClosePanel()));
    }

    private void Start()
    {
        originalParent = panelAllFile.transform.parent.gameObject;
        originalLocalPosition = panelAllFile.transform.localPosition;
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
        if (!IsThisFilePaperOpened() || panelAnimating || panelOpened) return;
        
        canvas.sortingOrder = 2;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosX(-5, 0.3f);
    }

    private void OnPointerExit(BaseEventData data)
    {
        if (!IsThisFilePaperOpened() || panelAnimating || panelOpened) return;
        
        canvas.sortingOrder = 1;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosX(0, 0.3f);
    }
    
    private void OnPointerDown(BaseEventData data)
    {
        Debug.Log("OnPointerDown");
    }

    private void OnPointerUp(BaseEventData data)
    {
        StartCoroutine(OpenPanel());
    }

    private IEnumerator OpenPanel()
    {
        if (!IsThisFilePaperOpened() || panelAnimating || panelOpened) yield break;

        panelAnimating = true;

        panelOpened = true;
        
        panelFileLetter.DOKill();
        panelFileLetter.DOAnchorPosX(0, 0.3f);
        
        panelAllFile.transform.SetParent(FilePaperManager.instance.panelContainerOpen.transform);
        panelAllFile.transform.DOLocalMove(Vector2.zero, 0.3f);
        panelAllFile.transform.DOScale(4, 0.3f);

        allPanelCanvas.sortingOrder = 10;
        allPanelCanvas.overrideSorting = true;

        yield return new WaitForSeconds(0.3f);

        panelAnimating = false;
    }
    
    private IEnumerator ClosePanel()
    {
        if (!IsThisFilePaperOpened() || panelAnimating || !panelOpened) yield break;

        panelAnimating = true;

        panelOpened = false;
        
        panelAllFile.transform.SetParent(originalParent.transform);
        panelAllFile.transform.DOLocalMove(originalLocalPosition, 0.3f);
        panelAllFile.transform.DOScale(1, 0.3f);

        allPanelCanvas.sortingOrder = 1;
        allPanelCanvas.overrideSorting = true;

        yield return new WaitForSeconds(0.3f);

        panelAnimating = false;
    }
    
    private bool IsThisFilePaperOpened()
    {
        var filePaperButtonOpened = FilePaperManager.instance.GetFilePaperOpened();
        return filePaperButtonOpened == null || filePaperButtonOpened == this;
    }
}