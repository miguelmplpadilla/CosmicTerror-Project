using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject panelGlobalObjects;
    public RectTransform mouse;

    public string currentName = "Miguel";
    public string currentMotv = "Dolor de barriga";

    public string currentDefault = "";

    public int currentDefaultIndex = 0;

    private void Awake()
    {
        instance = this;
    }
}
