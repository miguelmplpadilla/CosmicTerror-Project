using System;
using System.Collections;
using DG.Tweening;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CorkBoardController : MonoBehaviour
{
    public Button openCloseButton;
    
    public RectTransform corkBoardRt;
    public RectTransform buttonOpenCloseRt;

    private bool isOpened = false;
    private bool isAnimating = false;

    private void Start()
    {
        openCloseButton.onClick.AddListener( () => StartCoroutine(OpenClose()));
    }

    private void OnDestroy()
    {
        openCloseButton.onClick.RemoveListener(() => StartCoroutine(OpenClose()));
    }

    public IEnumerator OpenClose()
    {
        if (isAnimating) yield break;
        
        isAnimating = true;
        isOpened = !isOpened;
        
        if (!isOpened) 
            EventBus<CheckInsideCorkboard>.Raise(new CheckInsideCorkboard());

        buttonOpenCloseRt.DOAnchorPosX(isOpened ? -9.6541f : 0, 1);
        corkBoardRt.DOAnchorPosX(isOpened ? 490.3541f : 0, 1);
        
        buttonOpenCloseRt.GetChild(0).localScale = new Vector3(1, isOpened ? -1 : 1, 1);

        yield return new WaitForSeconds(1.05f);

        isAnimating = false;
    }
}
