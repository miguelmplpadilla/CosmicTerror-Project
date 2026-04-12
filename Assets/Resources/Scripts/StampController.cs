using System;
using System.Collections;
using DG.Tweening;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class StampController : DragBaseManager
{
    public RectTransform rtTop;
    public Button buttonStamp;

    private bool _isAnimating = false;

    public GameObject prefabStamp;
    public BoxCollider2D boxCollider;
    
    public TypeStamp typeStamp;

    public enum TypeStamp
    {
        NONE, HOSPITAL, PSYCOLOGIST, POLICE
    }

    protected override void Awake()
    {
        base.Awake();
        
        buttonStamp.onClick.AddListener(() => StartCoroutine(Stamp()));
    }

    private void OnDestroy()
    {
        buttonStamp.onClick.RemoveListener(() => StartCoroutine(Stamp()));
    }

    private IEnumerator Stamp()
    {
        if (_isAnimating) yield break;
        _isAnimating = true;
        
        objBig.transform.GetChild(0).gameObject.SetActive(false);
        
        float originalPositionY = rtTop.anchoredPosition.y;
        
        rtTop.DOAnchorPosY(0, 0.1f);
        yield return new WaitForSeconds(0.1f);
        StartCoroutine(StampPaper());
        rtTop.DOAnchorPosY(originalPositionY, 0.2f);
        yield return new WaitForSeconds(0.2f);
        
        objBig.transform.GetChild(0).gameObject.SetActive(true);

        _isAnimating = false;
    }

    private IEnumerator StampPaper()
    {
        PaperDragManager paperDragManager = GetPaperDragManagerStamp();
        
        if (paperDragManager != null)
        {
            if (paperDragManager.typeStamp == TypeStamp.NONE) 
                paperDragManager.typeStamp = typeStamp;
            
            GameObject stampInstantiated = Instantiate(prefabStamp, GameManager.instance.panelGlobalObjects.transform);
            stampInstantiated.SetActive(false);
            stampInstantiated.transform.position = boxCollider.transform.position;
            
            yield return null;
            
            stampInstantiated.transform.SetParent(paperDragManager.stampsParent.transform);
            stampInstantiated.SetActive(true);
        }
    }

    private PaperDragManager GetPaperDragManagerStamp()
    {
        RaycastHit2D[] raycastHits =
            Physics2D.BoxCastAll(boxCollider.transform.position, boxCollider.size, 0, Vector2.zero);

        foreach (var hit in raycastHits)
        {
            if (hit.collider.TryGetComponent(out PaperDragManager paperDragManager))
            {
                return paperDragManager;
            }
        }
        
        return null;
    }
}
