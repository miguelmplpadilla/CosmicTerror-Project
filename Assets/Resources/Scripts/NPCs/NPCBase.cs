using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCBase : MonoBehaviour
{
    public Animator animator;
    public CanvasGroup canvasGroup;

    public string npcName;
    public LocalizableString motiveText;
    
    public KnownDocument[] knownDocuments;

    protected virtual void Start()
    {
        npcName = GameManager.instance.npcName.namesList[Random.Range(0, GameManager.instance.npcName.namesList.Count)] + " " + 
                  GameManager.instance.npcName.lastNamesList[Random.Range(0, GameManager.instance.npcName.lastNamesList.Count)];
        
        GameManager.instance.currentName = npcName;
        GameManager.instance.currentMotv = motiveText.value; //TODO: Modificar motivos mediante DialogueNode
    }

    public void AskObject(DocumentData documentData)
    {
        DialogueController.instance.StartDialogue(documentData.GetDialogue(this), this, 0);
    }
}

[Serializable]
public class KnownDocument
{
    public GameObject document;
    public DialogueCreator dialogueDocument;
}
