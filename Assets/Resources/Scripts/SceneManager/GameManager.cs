using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public List<DialogueCreator> dialoguesPlayedConnection = new List<DialogueCreator>();
    
    public NamesNpc npcName;
    
    public GameObject panelGlobalObjects;
    public RectTransform mouse;

    public DragBaseManager currentDraggingObject;

    public string currentName = "Miguel";
    public string currentMotv = "Dolor de barriga";

    public string currentDefault = "";

    public int currentDefaultIndex = 0;

    public bool isNPCShowed = false;

    private void Awake()
    {
        instance = this;

        foreach (var nameNpc in npcName.names.Replace(" ", "").Split(","))
            npcName.namesList.Add(nameNpc);
        foreach (var lastName in npcName.lastName.Replace(" ", "").Split(","))
            npcName.lastNamesList.Add(lastName);
    }

    public void AddDialogueCreatorConnection(DialogueCreator dialogueCreator)
    {
        dialoguesPlayedConnection.Add(dialogueCreator);
    }

    public bool CheckIfDialogueConnectionPlayed(DialogueCreator dialogueCreator)
    {
        return dialoguesPlayedConnection.Find(it => it == dialogueCreator) != null;
    }
}

[Serializable]
public class NamesNpc
{
    public string names;
    public string lastName;

    [NonSerialized] public List<string> namesList = new List<string>();
    [NonSerialized] public List<string> lastNamesList = new List<string>();
}
