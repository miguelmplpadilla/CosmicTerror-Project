using System.Collections;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class FilePaperInterManager : InterBaseController
{
    public GameObject prefabDocumentFile;
    public GameObject prefabPhotoFile;

    public GameObject container;
    public GameObject containerScroll;
    public GameObject scrollPanel;
    
    public VerticalLayoutGroup containerVerticalLayout;

    public BoxCollider2D boxCollider;
    public RectTransform rt;
    
    public List<GameObject> documentFiles = new List<GameObject>();

    public FilePaperButton filePaperButton;

    private void Update()
    {
        boxCollider.size = rt.sizeDelta;
    }

    public override void Inter(DocumentBaseController documentBaseController)
    {
        containerVerticalLayout.enabled = false;
        
        GameObject finalObj = null;
        switch (documentBaseController)
        {
            case PaperDragManager paperDragManager:
                finalObj = InstancePaper(paperDragManager);
                break;
            case PhotoController photoController:
                finalObj = InstancePhoto(photoController);
                break;
        }
        
        finalObj?.SetActive(false);

        if (finalObj != null)
        {
            finalObj.GetComponent<FileDragController>().filePaperInterManager = this;
            documentFiles.Add(finalObj);
            documentBaseController.gameObject.SetActive(false);
            
            container.SetActive(documentFiles.Count <= 12);
            scrollPanel.SetActive(documentFiles.Count > 12);
            
            var correctContainer = documentFiles.Count <= 12 ? container : containerScroll;
            
            foreach (var documentFile in documentFiles)
                documentFile.transform.SetParent(correctContainer.transform);
        }
        
        StartCoroutine(RefreshLayoutNextFrame(finalObj));
    }
    
    private IEnumerator RefreshLayoutNextFrame(GameObject objCreated)
    {
        yield return new WaitForEndOfFrame();
        containerVerticalLayout.enabled = true;
        objCreated?.SetActive(true);
    }

    public GameObject InstancePaper(PaperDragManager paperDragManager)
    {
        var documentInstance = Instantiate(prefabDocumentFile, container.transform);

        var documentFile = documentInstance.GetComponent<PaperFile>();
        documentFile.SetData(paperDragManager);

        return documentInstance;
    }
    
    public GameObject InstancePhoto(PhotoController photoController)
    {
        var photoInstance = Instantiate(prefabPhotoFile, container.transform);

        var documentFile = photoInstance.GetComponent<PhotoFile>();
        documentFile.SetData(photoController);

        return photoInstance;
    }

    public override bool CanInteractWith(DragBaseManager objInter)
    {
        var canBase = base.CanInteractWith(objInter);

        return canBase && filePaperButton.panelOpened;
    }

    public override void ShowIcon(bool show)
    {
        MouseController.instance.ShowAddIcon(show);
    }
}