using System;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;

public class ConnectionsController : MonoBehaviour
{
    public static ConnectionsController instance;

    public DocumentConnectionCreator documentConnectionCreator;

    public GameObject prefabLine;

    public List<LineFallController> lines = new List<LineFallController>();

    private void Awake()
    {
        instance = this;
        
        RectTransform rectTransform = transform as RectTransform;
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.offsetMin = Vector2.zero;
            rectTransform.offsetMax = Vector2.zero;
        }
    }

    public LineFallController CreateLine(RectTransform pointA, RectTransform pointB)
    {
        GameObject lineInst = Instantiate(prefabLine, transform);
        
        LineFallController lineController = lineInst.GetComponent<LineFallController>();
        lineController.SetPoints(pointA, pointB);
        
        lines.Add(lineController);

        return lineController;
    }
    
    public void RemoveLine(LineFallController line)
    {
        lines.Remove(line);
        Destroy(line.gameObject);
    }

    public bool CheckCorrectConnection(DocumentData documentA, DocumentData documentB)
    {
        if (documentA == null || documentB == null) return false;
        
        var documentConnection =
            documentConnectionCreator.nodes.Find(it => (it as DocumentConectionNode)?.documentData == documentA) as DocumentConectionNode;

        var outputPort = documentConnection.GetOutputPort(nameof(documentConnection.baseOutput));
        DocumentConectionNode outputDocument = outputPort?.Connection?.node as DocumentConectionNode;
        
        var inputPort = documentConnection.GetInputPort(nameof(documentConnection.baseInput));
        DocumentConectionNode inputDocument = inputPort?.Connection?.node as DocumentConectionNode;

        if (outputDocument?.documentData == documentB) return true;
        if (inputDocument?.documentData == documentB) return true;

        return false;
    }
}
