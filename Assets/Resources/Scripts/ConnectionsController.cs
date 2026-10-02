using System;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using XNode;

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

    public bool CheckIfDocumentsAreConnected(DocumentBaseController documentA, DocumentBaseController documentB)
    {
        foreach (var lineFallController in lines)
        {
            if ((lineFallController.documentA == documentA && lineFallController.documentB == documentB) ||
                (lineFallController.documentA == documentB && lineFallController.documentB == documentA))
                return true;
        }

        return false;
    }

    public bool CheckCorrectConnection(DocumentData documentA, DocumentData documentB)
    {
        if (documentA == null || documentB == null) return false;

        Debug.Log("Document: "+documentA);
        
        var documentConnection =
            documentConnectionCreator.nodes.Find(it => (it as DocumentConnectionNode)?.documentData == documentA) as DocumentConnectionNode;

        if (documentConnection == null) return false;

        var outputPort = documentConnection.GetOutputPort(nameof(documentConnection.baseOutput));
        DocumentConnectionNode outputDocument = null;
        if (outputPort is { IsConnected: true }) outputDocument = GetOutputConnectionNode(outputPort);
        
        var inputPort = documentConnection.GetInputPort(nameof(documentConnection.baseInput));
        DocumentConnectionNode inputDocument = null;
        if (inputPort is { IsConnected: true }) inputDocument = GetInputConnectionNode(inputPort);

        if (outputDocument?.documentData == documentB) return true;
        if (inputDocument?.documentData == documentB) return true;

        return false;
    }

    private DocumentConnectionNode GetOutputConnectionNode(NodePort port)
    {
        IntermediateDocumentConnectionNode connectionNode = port?.Connection?.node as IntermediateDocumentConnectionNode;
        
        if (connectionNode == null) return null;
        
        var outputPort = connectionNode.GetOutputPort(nameof(connectionNode.baseOutput));
        DocumentConnectionNode outputDocument = outputPort?.Connection?.node as DocumentConnectionNode;

        return outputDocument;
    }
    
    private DocumentConnectionNode GetInputConnectionNode(NodePort port)
    {
        IntermediateDocumentConnectionNode connectionNode = port?.Connection?.node as IntermediateDocumentConnectionNode;

        if (connectionNode == null) return null;
        
        var inputPort = connectionNode.GetInputPort(nameof(connectionNode.baseInput));
        DocumentConnectionNode inputDocument = inputPort?.Connection?.node as DocumentConnectionNode;

        return inputDocument;
    }

    public DialogueCreator GetDialogueConnection(DocumentData documentA, DocumentData documentB)
    {
        if (documentA == null) return null;
        
        var documentConnection =
            documentConnectionCreator.nodes.Find(it => (it as DocumentConnectionNode)?.documentData == documentA) as DocumentConnectionNode;

        if (documentConnection == null) return null;

        var outputPort = documentConnection.GetOutputPort(nameof(documentConnection.baseOutput));
        DocumentConnectionNode outputDocument = null;
        if (outputPort is { IsConnected: true }) outputDocument = GetOutputConnectionNode(outputPort);
        
        var inputPort = documentConnection.GetInputPort(nameof(documentConnection.baseInput));
        DocumentConnectionNode inputDocument = null;
        if (inputPort is { IsConnected: true }) inputDocument = GetInputConnectionNode(inputPort);

        if (outputDocument?.documentData == documentB) return GetIntermediateConnectionNode(outputPort)?.dialogueConnection;
        if (inputDocument?.documentData == documentB) return GetIntermediateConnectionNode(inputPort)?.dialogueConnection;

        return null;
    }

    private IntermediateDocumentConnectionNode GetIntermediateConnectionNode(NodePort port)
    {
        IntermediateDocumentConnectionNode connectionNode = port?.Connection?.node as IntermediateDocumentConnectionNode;

        return connectionNode;
    }
}
