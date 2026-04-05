using System;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;
using UnityEngine.EventSystems;

public class PushpinController : MonoBehaviour
{
    public List<LineController> currentLines = new List<LineController>();

    public RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    public void OnBeginDrag()
    {
        currentLines.Add(ConnectionsController.instance.CreateLine(rt, GameManager.instance.mouse));
    }

    public void OnEndDrag()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, results);

        foreach (var result in results)
        {
            if (result.gameObject.TryGetComponent(out PaperDragManager dragPaper))
            {
                currentLines[currentLines.Count - 1].pointB = dragPaper.pushpin.rt;
                dragPaper.pushpin.currentLines.Add(currentLines[currentLines.Count - 1]);
                return;
            }
        }
        
        ConnectionsController.instance.RemoveLine(currentLines[currentLines.Count - 1]);
    }

    public void RemoveAllLines()
    {
        foreach (var line in currentLines)
            ConnectionsController.instance.RemoveLine(line);
        
        currentLines.Clear();
    }
}
