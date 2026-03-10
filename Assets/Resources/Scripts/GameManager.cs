using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public GameObject panelGlobalObjects;

    private void Awake()
    {
        instance = this;
    }
}
