using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject panelGlobalObjects;
    public RectTransform mouse;

    private void Awake()
    {
        instance = this;
    }
}
