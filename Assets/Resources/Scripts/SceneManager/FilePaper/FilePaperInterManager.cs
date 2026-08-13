using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;

public class FilePaperInterManager : InterBaseController
{
    public GameObject prefabDocumentFile;
    public GameObject prefabPhotoFile;

    public GameObject container;

    public BoxCollider2D boxCollider;
    public RectTransform rt;
    
    public List<GameObject> documentFiles = new List<GameObject>();

    private void Update()
    {
        boxCollider.size = rt.sizeDelta;
        boxCollider.offset = new Vector2(rt.sizeDelta.x/2, -rt.sizeDelta.y/2);
    }

    public override void Inter(DocumentData documentData)
    {
        GameObject finalObj = null;
        switch (documentData.documentBaseController)
        {
            case PaperDragManager paperDragManager:
                finalObj = InstancePaper(paperDragManager);
                break;
            case PhotoController photoController:
                finalObj = InstancePhoto(photoController);
                break;
        }

        if (finalObj != null)
        {
            documentFiles.Add(finalObj);
            documentData.documentBaseController.gameObject.SetActive(false);
        }
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
}