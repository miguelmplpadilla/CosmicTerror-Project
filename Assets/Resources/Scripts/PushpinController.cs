using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class PushpinController : MonoBehaviour
{
    public List<LineFallController> currentLines = new List<LineFallController>();
    private LineFallController currentLine;

    public DocumentBaseController documentBaseController;

    public RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void OnBeginDrag()
    {
        currentLine = ConnectionsController.instance.CreateLine(rt, GameManager.instance.mouse);
        currentLines.Add(currentLine);

        currentLine.connecting = true;
    }

    public void OnDrag()
    {
        if (currentLine == null) return;
        
        DocumentBaseController nearestCorrectDocument = GetCorrectNearDocument();
        if (nearestCorrectDocument == null ||
            ConnectionsController.instance.CheckIfDocumentsAreConnected(documentBaseController, nearestCorrectDocument))
            return;
        
        float distance = Vector3.Distance(MouseController.instance.transform.position,  nearestCorrectDocument.transform.position);

        if (distance < 300)
        {
            float cantTransition = (distance / 300) - 0.3f;
            if (cantTransition < 0) cantTransition = 0;
            if (distance < 90) cantTransition = 0;
            
            currentLine.lineRenderer.color = Color.Lerp(Color.red, Color.black, cantTransition);
            return;
        }
        
        currentLine.lineRenderer.color = Color.black;
    }

    public void OnEndDrag()
    {
        currentLine.connecting = false;
        
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent(out DocumentBaseController document) && result.gameObject.transform.parent.name.Equals("Cork"))
            {
                if (ConnectionsController.instance.CheckIfDocumentsAreConnected(documentBaseController, document))
                    break;
                
                bool isCorrectConnection = ConnectionsController.instance.CheckCorrectConnection(
                    documentBaseController.documentData,
                    document.documentData);

                currentLine.lineRenderer.color = isCorrectConnection ? Color.red : Color.black;
                currentLine.pointB = document.pushpin.rt;

                StartCoroutine(OnSuccessFailLine(isCorrectConnection));

                if (!isCorrectConnection) return;
                
                var dialogueCreator = ConnectionsController.instance.GetDialogueConnection(
                    documentBaseController.documentData, document.documentData);

                if (!GameManager.instance.CheckIfDialogueConnectionPlayed(dialogueCreator))
                {
                    StartCoroutine(DialogueController.instance.StartDialogue(dialogueCreator, null));
                    GameManager.instance.AddDialogueCreatorConnection(dialogueCreator);
                }

                currentLine.documentA = documentBaseController;
                currentLine.documentB = document;
                
                document.pushpin.currentLines.Add(currentLine);
                return;
            }
        }
        
        ConnectionsController.instance.RemoveLine(currentLine);
    }

    private IEnumerator OnSuccessFailLine(bool success)
    {
        var tween = currentLine.transform.DOPunchScale(Vector3.one * 0.6f, 0.3f).SetLoops(success ? 1 : 2);

        yield return tween.WaitForCompletion();
        
        if (!success) ConnectionsController.instance.RemoveLine(currentLine);
    }

    public void RemoveAllLines()
    {
        foreach (var line in currentLines)
        {
            if (line == null) continue; 
            ConnectionsController.instance.RemoveLine(line);
        }
        
        currentLines.Clear();
    }

    public void ChangeAlpha(float alpha)
    {
        foreach (var line in currentLines)
        {
            if (line == null) continue;
            Color color = line.lineRenderer.color;
            color.a = alpha;
            line.lineRenderer.color = color;
        }
    }

    public DocumentBaseController GetCorrectNearDocument()
    {
        List<DocumentBaseController> documentObjs = FindObjectsByType<DocumentBaseController>(FindObjectsSortMode.None).ToList();

        float distanceNearest = 100000000000000000000f;
        DocumentBaseController nearestDocument = null;

        foreach (var documentObj in documentObjs)
        {
            if (documentObj == documentBaseController) continue;
            
            var isCorrect = ConnectionsController.instance.CheckCorrectConnection(documentBaseController.documentData,
                documentObj.documentData);
            if (!isCorrect) continue;

            float distance =
                Vector3.Distance(documentBaseController.transform.position, documentObj.transform.position);
            if (distance < distanceNearest)
                nearestDocument = documentObj;
        }

        return nearestDocument;
    }
}
