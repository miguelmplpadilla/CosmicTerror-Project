using System;
using System.Collections.Generic;
using Resources.Scripts;
using UnityEngine;

public class ConnectionsController : MonoBehaviour
{
    public static ConnectionsController instance;

    public GameObject prefabLine;

    public List<LineController> lines = new List<LineController>();

    private void Awake()
    {
        instance = this;
    }

    public LineController CreateLine(RectTransform pointA, RectTransform pointB)
    {
        GameObject lineInst = Instantiate(prefabLine, transform);
        
        LineController lineController = lineInst.GetComponent<LineController>();
        lineController.SetPoints(pointA, pointB);
        
        lines.Add(lineController);

        return lineController;
    }
    
    public void RemoveLine(LineController line)
    {
        lines.Remove(line);
        Destroy(line.gameObject);
    }
}
